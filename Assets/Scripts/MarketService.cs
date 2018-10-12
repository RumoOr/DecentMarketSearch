using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketService
{
    private const string BASE_URL = "https://api.decentraland.org/";

    public const string PARCEL_DETAIL_URL = "https://market.decentraland.org/{0}/{1}/detail";

    private const string API_VERSION = "v1/";

    private const string API_PARCELS = "parcels";

    private const string API_MAP = "parcels/{0}/{1}/map.png";

    private const string ARG_STATUS = "status";
    private const string ARG_SORT_BY = "sort_by";
    private const string ARG_SORT_ORDER = "sort_order";
    private const string ARG_LIMIT = "limit";
    private const string ARG_OFFSET = "offset";

    private const string ARG_MAP_WIDTH = "width";
    private const string ARG_MAP_HEIGHT = "height";
    private const string ARG_MAP_SIZE = "size";
    private const string ARG_MAP_PUBLICATIONS = "publications";

    private const string VALUE_STATUS = "open";         // open, cancelled or sold 
    private const string VALUE_SORT_PRICE = "price";    // price, created_at, block_time_updated_at or expires_at
    private const string VALUE_SORT_ORDER = "asc";      // asc or desc
    private const int VALUE_LIMIT = 20;                 // 10 - 20

    private const int VALUE_MAP_WIDTH = 200;
    private const int VALUE_MAP_HEIGHT = 100;
    private const int VALUE_MAP_SIZE = 10;
    private const bool VALUE_MAP_PUBLICATIONS = true;

    public static string GetDetailUrl(Parcel parcel)
    {
        return string.Format(PARCEL_DETAIL_URL,
            parcel.x, parcel.y);
    }

    public static string GetMapUrl(Parcel parcel)
    {
        return GetUrl(
            string.Format(API_MAP, parcel.x, parcel.y),
            new[] { ARG_MAP_WIDTH, VALUE_MAP_WIDTH.ToString() },
            new[] { ARG_MAP_HEIGHT, VALUE_MAP_HEIGHT.ToString() },
            new[] { ARG_MAP_SIZE, VALUE_MAP_SIZE.ToString() },
            new[] { ARG_MAP_PUBLICATIONS, VALUE_MAP_PUBLICATIONS.ToString().ToLower() });
    }

    private static string GetUrl(string path = null, params string[][] queue)
    {
        var uri = BASE_URL + API_VERSION + (path ?? (""));

        var append = "";
        for (var i = 0; i < queue.Length; i++)
        {
            append += i == 0 ? "?" : "&";
            append += queue[i][0] + "=" + queue[i][1];
        }

        return uri + append;
    }

    public IEnumerator GetParcels(Action<Parcels, int, int> callback)
    {
        var page = 0;
        var pageCount = 1;

        while (page < pageCount)
        {
            var uri = GetUrl(
                API_PARCELS,
                new[] { ARG_STATUS, VALUE_STATUS },
                new[] { ARG_OFFSET, "" + (page * VALUE_LIMIT) },
                new[] { ARG_SORT_BY, VALUE_SORT_PRICE },
                new[] { ARG_SORT_ORDER, VALUE_SORT_ORDER },
                new[] { ARG_LIMIT, VALUE_LIMIT.ToString() });

            using (var www = new WWW(uri))
            {
                yield return www;

                Parcels result = null;
                try
                {
                    result = JsonUtility.FromJson<Parcels>(www.text);

                    if (page == 0)
                    {
                        pageCount = (result.data.total / VALUE_LIMIT) +
                            (result.data.total % VALUE_LIMIT == 0 ? 0 : 1);

                        // ToDo: only for debug
                        //pageCount = 5;
                    }
                }
                catch (Exception e)
                {
                    page = 0;
                    pageCount = 1;
                   
                    Debug.Log(e.StackTrace);
                }

                callback.Invoke(result, page, pageCount);

                page++;
            }
        }
    }

    public IEnumerator GetMap(Parcel parcel, Action<Parcel, Texture> callback)
    {
        using (var www = new WWW(GetMapUrl(parcel)))
        {
            yield return www;

            callback.Invoke(parcel, www.texture);
        }
    }
}
