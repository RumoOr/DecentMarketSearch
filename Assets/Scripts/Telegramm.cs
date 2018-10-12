using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telegramm : MonoBehaviour, ISender
{
    private const string BASE_URL = "https://api.telegram.org/bot{0}/sendMessage";

    private const string ARG_CHAT_ID = "chat_id";
    private const string ARG_TEXT = "text";

    public string m_APIToken;

    public string m_ChatId;

    public void Send(Parcel parcel)
    {
        if (parcel.Hot < 200)
            return;

        var message = string.Format("Parcel [{0},{1}]", parcel.x, parcel.y);
        message += " [" + parcel.Hot + "]";
        message += " [mana: " + parcel.Price + "]";
        if (parcel.tags.proximity.HasRoad)
            message += " [road: " + parcel.RoadDistance + "]";
        if (parcel.tags.proximity.HasDistrict)
            message += " [dist: " + parcel.DistrictDistance + "]";
        if (parcel.tags.proximity.HasPlaza)
            message += " [plaza: " + parcel.PlazaDistance + "]";
        message += "\n" + MarketService.GetDetailUrl(parcel);
        message += "\n" + MarketService.GetMapUrl(parcel);

        StartCoroutine(SendRoutine(message));
    }

    private string GetUrl(string message)
    {
        return string.Format(BASE_URL, m_APIToken) +
            "?" + ARG_CHAT_ID + "=" + m_ChatId +
            "&" + ARG_TEXT + "=" + message;
    }

    private IEnumerator SendRoutine(string message)
    {
        using (var www = new WWW(GetUrl(message)))
        {
            yield return www;
        }
    }
}
