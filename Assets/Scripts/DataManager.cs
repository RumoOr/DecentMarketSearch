using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager s_Instance;

    public static DataManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new DataManager();
                s_Instance.Init();
            }
            return s_Instance;
        }
    }

    private readonly List<Parcel> m_ParcelBuffer = new List<Parcel>();

    public Parcel[] Parcels { get; private set; }

    public int Count { get { return Parcels == null ? 0 : Parcels.Length; } }

    public bool IsLoading { get; private set; }

    public Parcel this[int index]
    {
        get { return Count > index ? Parcels[index] : null; }
    }

    private Action<int, int> m_OnDataLoadedListener;

    private MarketService m_Service;

    private IRating m_Rating;

    void Init()
    {
        m_Service = new MarketService();

        m_Rating = new MLRegRating();
    }

    private void OnParcelsUpdate(Parcels parcels, int page, int pageCount)
    {
        if (parcels != null)
        {
            for (var i = 0; i < parcels.data.parcels.Count; i++)
            {
                parcels.data.parcels[i].index =
                    (page * parcels.data.parcels.Count) + i;

                m_Rating.AddValues(parcels.data.parcels[i]);
            }

            m_ParcelBuffer.AddRange(parcels.data.parcels);
        }

        if (page == pageCount - 1)
        {
            m_Rating.Learn();

            foreach (var parcel in m_ParcelBuffer)
            {
                parcel.Hot = (int)(((float)(m_Rating.GetRating(parcel) -
                    parcel.Price) / parcel.Price) * 100);
            }

            Parcels = m_ParcelBuffer.Select(p => p).ToArray();

            m_ParcelBuffer.Clear();

            IsLoading = false;
        }

        if (m_OnDataLoadedListener != null)
            m_OnDataLoadedListener.Invoke(page, pageCount);
    }

    public void DoUpdate(MonoBehaviour context,
        Action<int, int> onDataLoadedListener = null)
    {
        if (IsLoading)
            return;

        IsLoading = true;

        m_OnDataLoadedListener = onDataLoadedListener;

        m_Rating.Reset();

        context.StartCoroutine(m_Service.GetParcels(OnParcelsUpdate));
    }

    public void GetMap(MonoBehaviour context, Parcel parcel, Action<Parcel,
        Texture> onMapLoadedListener)
    {
        context.StartCoroutine(m_Service.GetMap(parcel, onMapLoadedListener));
    }
}
