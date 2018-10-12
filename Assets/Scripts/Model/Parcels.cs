using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Data2
{
    public int version;
    public string name;
    public string description;
    public string ipns;
}

[Serializable]
public class Road
{
    public string district_id;
    public int distance;

    public override string ToString()
    {
        return string.Format("[" +
            "id: {0}, " +
            "distance: {1}" + "]",
            district_id,
            distance);
    }
}

[Serializable]
public class Plaza
{
    public string district_id;
    public int distance;

    public override string ToString()
    {
        return string.Format("[" +
            "id: {0}, " +
            "distance: {1}" + "]",
            district_id,
            distance);
    }
}

[Serializable]
public class District
{
    public string district_id;
    public int distance;

    public override string ToString()
    {
        return string.Format("[" +
            "id: {0}, " +
            "distance: {1}" + "]",
            district_id,
            distance);
    }
}

[Serializable]
public class Proximity
{
    public Road road;
    public Plaza plaza;
    public District district;

    public bool HasRoad
    {
        get { return road != null && !string.IsNullOrEmpty(road.district_id); }
    }

    public bool HasPlaza
    {
        get { return plaza != null && !string.IsNullOrEmpty(plaza.district_id); }
    }

    public bool HasDistrict
    {
        get { return district != null && !string.IsNullOrEmpty(district.district_id); }
    }

    public override string ToString()
    {
        return "" +
            (HasRoad ? "road: " + road.ToString() : "") +
            (HasPlaza ? (HasRoad ? ", " : "") + "plaza: " + plaza.ToString() : "") +
            (HasDistrict ? (HasRoad || HasPlaza ? ", " : "") + "district: " + district.ToString() : "");
    }
}

[Serializable]
public class Tags
{
    public Proximity proximity;

    public override string ToString()
    {
        return "[" + proximity.ToString() + "]";
    }
}

[Serializable]
public class Publication
{
    public string tx_hash;
    public string tx_status;
    public string owner;
    public long price;
    public long expires_at;
    public string status;
    public object buyer;
    public string contract_id;
    public int block_number;
    public object block_time_created_at;
    public object block_time_updated_at;
    public string type;
    public string asset_id;
    public string marketplace_id;
}

[Serializable]
public class Parcel
{
    private const int PROXIMITY_DISTANCE_MAX = 10;

    public int index;
    public string id;
    public int x;
    public int y;
    public int auction_price;
    public object district_id;
    public string owner;
    public Data2 data;
    public string auction_owner;
    public Tags tags;
    public string last_transferred_at;
    public bool in_estate;
    public Publication publication;

    public int Hot { get; set; }

    public long Price
    {
        get { return publication.price; }
    }

    public int RoadDistance
    {
        get
        {
            return tags.proximity.HasRoad ?
                tags.proximity.road.distance : 
                PROXIMITY_DISTANCE_MAX;
        }
    }

    public int DistrictDistance
    {
        get
        {
            return tags.proximity.HasDistrict ?
                tags.proximity.district.distance : 
                PROXIMITY_DISTANCE_MAX;
        }
    }

    public int PlazaDistance
    {
        get
        {
            return tags.proximity.HasPlaza ?
                tags.proximity.plaza.distance :
                PROXIMITY_DISTANCE_MAX;
        }
    }

    public float Distance
    {
        get { return Vector2.Distance(Vector2.zero, new Vector2(x, y)); }
    }

    public override string ToString()
    {
        return string.Format("[" +
            "id: {0}, " +
            "position: {1}, {2}, " +
            "tags: {5}, " +
            "price: {3}, " +
            "owner: {4}" + "]",
            id,
            x, y,
            auction_price,
            owner,
            tags);
    }
}

[Serializable]
public class Data
{
    public List<Parcel> parcels;
    public int total;

    public override string ToString()
    {
        var str = "";
        for (var i = 0; i < parcels.Count; i++)
        {
            str += i > 0 ? ", " : "";
            str += parcels[i].ToString();
        }
        return string.Format("[" +
            "total: {0}, " +
            "parcels: [{1}]" + "]",
            total,
            str);
    }
}

[Serializable]
public class Parcels
{
    public bool ok;
    public Data data;

    public override string ToString()
    {
        return data.ToString();
    }
}