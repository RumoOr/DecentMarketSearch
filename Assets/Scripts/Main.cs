using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private const int VIEW_LOADING = 0;
    private const int VIEW_MARKET = 1;

    private const float DEFAULT_AUTO_REFRESH_INTERVAL = 60 * 3;

    private const int NOTEABLE_HOT_MIN = 200;

    private const int NOTEABLE_PRICE_MAX = 7000;

    private DataManager m_DataManager;

    private int m_CurrentView = -1;

    public bool m_AutoRefresh = false;

    public bool m_SendNotables = false;

    public float m_AutoRefreshInterval =
        DEFAULT_AUTO_REFRESH_INTERVAL;

    private ISender[] m_Senders;

    private readonly List<string> m_History = new List<string>();

    private float m_LatestAutoRefreshTime;

    void Start()
    {
        m_DataManager = DataManager.Instance;

        UI.Instance.Find<UIFilter>().OnExpandChangeEvent += OnFilterExpandedChanged;
        UI.Instance.Find<UIOrder>().OnExpandChangeEvent += OnFilterExpandedChanged;

        UI.Instance.Find<UIFilter>().OnApplyClickEvent += OnFilterApplyClicked;
        UI.Instance.Find<UIOrder>().OnApplyClickEvent += OnFilterApplyClicked;

        m_Senders = FindObjectsOfType<MonoBehaviour>()
            .Where(b => b is ISender)
            .Select(b => (ISender)b)
            .ToArray();

        NotifyDataSetChanged();
    }

    private void Update()
    {
        if (m_AutoRefresh &&
            Time.time - m_LatestAutoRefreshTime >= m_AutoRefreshInterval)
        {
            m_LatestAutoRefreshTime = Time.time;

            NotifyDataSetChanged();
        }
    }

    public void OnDataUpdate(int page, int pageCount)
    {
        UI.Instance.Find<UILoading>().DoUpdate(page + 1, pageCount);

        if (page == pageCount - 1)
        {
            SetCurrentView(VIEW_MARKET);

            UpdateAuctions();

            UI.Instance.Find<UIFilter>().DoUpdate();

            if (m_SendNotables)
                NotifySender(m_DataManager.Parcels);

            m_LatestAutoRefreshTime = Time.time;

            if (!(m_AutoRefresh && m_AutoRefreshInterval == 0))
                UI.Instance.Find<UIMarket>().SetRefreshButtonVisibility(true);
        }
        else if (pageCount == 0)
            m_LatestAutoRefreshTime = Time.time;
    }

    public void OnParcelClicked(UIParcel parcel)
    {
        Application.OpenURL(MarketService.GetDetailUrl(parcel.Data));
    }

    public void OnRefreshClicked()
    {
        NotifyDataSetChanged();
    }

    public void OnFilterExpandedChanged(bool isExpanded)
    {
        if (!isExpanded)
            UpdateAuctions();
    }

    public void OnFilterApplyClicked()
    {
        UpdateAuctions();
    }

    private void SetCurrentView(int id)
    {
        if (m_CurrentView == id)
            return;

        m_CurrentView = id;

        UI.Instance.Find<UILoading>().gameObject.SetActive(
            m_CurrentView == VIEW_LOADING);

        UI.Instance.Find<UIMarket>().gameObject.SetActive(
            m_CurrentView == VIEW_MARKET);
    }

    private void NotifyDataSetChanged()
    {
        m_LatestAutoRefreshTime = float.MaxValue;

        if (DataManager.Instance.Count == 0)
            SetCurrentView(VIEW_LOADING);

        UI.Instance.Find<UIMarket>().SetRefreshButtonVisibility(false);

        UI.Instance.Find<UILoading>().DoUpdate(1, 1);

        m_DataManager.DoUpdate(this, OnDataUpdate);
    }

    private void UpdateAuctions()
    {
        var parcels = m_DataManager.Parcels;

        var filter = UI.Instance.Find<UIFilter>().Data;
        if (filter != null)
        {
            if (filter.UsePrice)
            {
                parcels = parcels
                    .Where(p => p.publication.price >= filter.PriceMinValue &&
                        p.publication.price <= filter.PriceMaxValue)
                    .ToArray();
            }
            if (filter.UseRoad)
            {
                parcels = parcels
                    .Where(p => p.tags.proximity.HasRoad &&
                        p.tags.proximity.road.distance <= filter.RoadMaxDist)
                    .ToArray();
            }
            if (filter.UseDistrict)
            {
                parcels = parcels
                    .Where(p => p.tags.proximity.HasDistrict &&
                        p.tags.proximity.district.distance <= filter.DistrictMaxDist)
                    .ToArray();
            }
            if (filter.UsePlaza)
            {
                parcels = parcels
                    .Where(p => p.tags.proximity.HasPlaza &&
                        p.tags.proximity.plaza.distance <= filter.PlazaMaxDist)
                    .ToArray();
            }
            if (filter.UseHot)
            {
                parcels = parcels
                    .Where(p => p.Hot > 0)
                    .ToArray();
            }
        }

        var order = UI.Instance.Find<UIOrder>().Data;
        if (order != null)
        {
            var ordered = parcels
                .OrderBy(p => order.UseRoad ?
                    p.RoadDistance :
                    order.UseDistrict ?
                        p.DistrictDistance :
                        order.UsePlaza ?
                            p.PlazaDistance :
                            0);

            if (order.UsePrice)
                ordered = ordered.ThenBy(p => p.Price);
            else if (order.UseDate)
                ordered = ordered.ThenBy(p => p.publication.expires_at);
            else if (order.UseDistance)
                ordered = ordered.ThenBy(p => p.Distance);
            else if (order.UseHot)
                ordered = ordered.ThenByDescending(p => p.Hot);

            parcels = ordered.ToArray();
        }

        UI.Instance.Find<UIMarket>().DoUpdate(parcels,
            OnParcelClicked, OnRefreshClicked);
    }

    private string GetHistoryId(Parcel parcel)
    {
        return string.Format("{0};{1};{2}", parcel.x, parcel.y, parcel.Price);
    }

    private void NotifySender(Parcel[] parcels)
    {
        var noteableParcels = parcels
            .Where(p => p.Hot >= NOTEABLE_HOT_MIN || p.Price <= NOTEABLE_PRICE_MAX)
            .Where(p => !m_History.Contains(GetHistoryId(p)));

        foreach (var parcel in noteableParcels)
        {
            m_History.Add(GetHistoryId(parcel));

            foreach (var sender in m_Senders)
                sender.Send(parcel);
        }
    }
}
