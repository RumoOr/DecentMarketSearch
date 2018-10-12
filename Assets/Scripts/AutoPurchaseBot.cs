using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoPurchaseBot : MonoBehaviour, ISender
{
    private readonly Queue<Parcel> m_Parcels = new Queue<Parcel>();

    public string m_ExternalScriptPath;

    private bool HasParcels { get { return m_Parcels.Count > 0; } }

    private void Update()
    {
        if (!HasParcels)
            return;

        Buy(m_Parcels.Dequeue());

        Application.Quit();
    }

    public void Send(Parcel parcel)
    {
        if (parcel.Price <= 3000)
        {
            m_Parcels.Enqueue(parcel);
        }
        else if (
            parcel.Price <= 6000 &&
            parcel.Hot >= 230)
        {
            m_Parcels.Enqueue(parcel);
        }
        else if (
            parcel.Price <= 7000 &&
            parcel.RoadDistance == 0)
        {
            m_Parcels.Enqueue(parcel);
        }
        else if (
            parcel.Price <= 6500 &&
            parcel.Hot >= 200 &&
            parcel.RoadDistance <= 2)
        {
            m_Parcels.Enqueue(parcel);
        }
        else if (
           parcel.Price <= 7500 &&
           parcel.Hot >= 230 &&
           parcel.RoadDistance <= 3 &&
           Mathf.Abs(parcel.x) <= 75 &&
           Mathf.Abs(parcel.y) <= 75)
        {
            m_Parcels.Enqueue(parcel);
        }
    }

    private void Buy(Parcel parcel)
    {
        Debug.Log("Buy: " + parcel);

        SendToConsole(
            parcel.x.ToString(),
            parcel.y.ToString(),
            parcel.tags.proximity.HasRoad ? parcel.RoadDistance.ToString() : "-1",
            parcel.tags.proximity.HasDistrict ? parcel.DistrictDistance.ToString() : "-1",
            parcel.tags.proximity.HasPlaza ? parcel.PlazaDistance.ToString() : "-1",
            parcel.Price.ToString());
    }

    private void SendToConsole(params string[] args)
    {
        var process = new System.Diagnostics.Process();
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
            FileName = m_ExternalScriptPath,
            Arguments = args.Aggregate((c, n) => c + " " + n)
        };
        process.StartInfo = startInfo;
        process.Start();
    }
}
