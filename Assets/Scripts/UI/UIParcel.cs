using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIParcel : UIComponent
{
    private const int TEXT_CHAR_COUNT_MIN = 10;

    private Text m_TextName;

    private Text m_TextPrice;

    private Text m_TextDate;

    private Text m_TextPosition;

    private Text m_TextDistrict;

    private Text m_TextPlaza;

    private Text m_TextRoad;

    private Text m_TextHot;

    private Image m_ImageMap;

    private Button m_Button;

    public Sprite m_MapSprite;

    public Parcel Data { get; private set; }

    public override void OnCreate()
    {
        base.OnCreate();

        var texts = GetComponentsInChildren<Text>(true);

        m_TextName = texts.First(t => t.name == "Name");

        m_TextPrice = texts.First(t => t.name == "Price");

        m_TextDate = texts.First(t => t.name == "Date");

        m_TextPosition = texts.First(t => t.name == "Position");

        m_TextDistrict = texts.First(t => t.name == "District");

        m_TextPlaza = texts.First(t => t.name == "Plaza");

        m_TextRoad = texts.First(t => t.name == "Road");

        m_TextHot = texts.First(t => t.name == "Hot");

        var images = GetComponentsInChildren<Image>();

        m_ImageMap = images.First(i => i.name == "Map");

        m_Button = GetComponentInChildren<Button>(true);
    }

    public void DoUpdate(Parcel data, Action<UIParcel> onClickListener = null)
    {
        Data = data;

        m_ImageMap.sprite = m_MapSprite;

        var name = string.IsNullOrEmpty(data.data.name) ?
                "Parcel" :
                data.data.name;

        m_TextName.text = GetTextLength(name) < m_TextName.rectTransform.sizeDelta.x ?
            name :
            new string(name.ToCharArray().Take(TEXT_CHAR_COUNT_MIN).ToArray()) + "...";

        m_TextPrice.text =
            string.Format("{0:N0}", data.publication.price);

        var date = new DateTime(1970, 1, 1)
            .AddMilliseconds(data.publication.expires_at);
        var days = (int)(date - DateTime.Now).TotalDays;
        var hours = (int)(date - DateTime.Now).TotalHours;

        if (days > 30)
        {
            m_TextDate.text = string.Format("Expires in {0} {1}",
                days / 30, days / 30 > 1 ? "months" : "month");
        }
        else if (days < 1)
        {
            m_TextDate.text = string.Format("Expires in {0} {1}",
                hours, hours > 1 ? "hours" : "hour");
        }
        else
        {
            m_TextDate.text = string.Format("Expires in {0} {1}",
                days, days > 1 ? "days" : "day");
        }

        m_TextPosition.text = string.Format("{0}, {1}", data.x, data.y);

        m_TextDistrict.transform.parent.parent.gameObject.SetActive(data.tags.proximity.HasDistrict);
        if (data.tags.proximity.HasDistrict)
            m_TextDistrict.text = data.tags.proximity.district.distance.ToString();

        m_TextPlaza.transform.parent.parent.gameObject.SetActive(data.tags.proximity.HasPlaza);
        if (data.tags.proximity.HasPlaza)
            m_TextPlaza.text = data.tags.proximity.plaza.distance.ToString();

        m_TextRoad.transform.parent.parent.gameObject.SetActive(data.tags.proximity.HasRoad);
        if (data.tags.proximity.HasRoad)
            m_TextRoad.text = data.tags.proximity.road.distance.ToString();

        m_TextHot.text = string.Format("{0:N0}", data.Hot);
        m_TextHot.transform.parent.parent.gameObject.SetActive(data.Hot > 0);

        m_Button.onClick.RemoveAllListeners();

        if (onClickListener != null)
            m_Button.onClick.AddListener(() => onClickListener(this));
    }

    public void SetMap(Texture2D texture)
    {
        m_ImageMap.sprite = Sprite.Create(
             (Texture2D)texture,
             new Rect(0, 0, texture.width, texture.height),
             new Vector2(0.5f, 0.5f));
    }

    private int GetTextLength(string text)
    {
        var result = 0;

        var font = m_TextName.font;
        var cInfo = new CharacterInfo();

        var array = text.ToCharArray();
        foreach (char c in array)
        {
            font.GetCharacterInfo(c, out cInfo, m_TextName.fontSize);
            result += cInfo.advance;
        }
        return result;
    }
}
