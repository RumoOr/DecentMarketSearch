using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOrder : UISidePanel
{
    private const int RADIO_PRICE = 0;
    private const int RADIO_DATE = 1;
    private const int RADIO_DISTANCE = 2;
    private const int RADIO_HOT = 3;

    private const int RADIO_ROAD = 4;
    private const int RADIO_DISTRICT = 5;
    private const int RADIO_PLAZA = 6;

    private const int DEFAULT_ORDER = RADIO_HOT;

    public Color m_ColorToogleEnabled;
    public Color m_ColorToogleDisabled;

    private Toggle[] m_Radios;

    public Toggle this[int index] { get { return m_Radios[index]; } }

    public ParcelOrder Data { get; private set; }

    public override void OnCreate()
    {
        base.OnCreate();

        m_Radios = GetComponentsInChildren<Toggle>(true);
        var index = 0;
        foreach (var radio in m_Radios)
        {
            var id = index;
            index++;

            radio.onValueChanged.AddListener((bool enabled) =>
                OnRadioValueChanged(id, enabled));
            radio.isOn = id == DEFAULT_ORDER;
        }

        Data = new ParcelOrder();

        IsExpanded = true;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        Data.UsePrice = m_Radios[RADIO_PRICE].isOn;
        Data.UseDate = m_Radios[RADIO_DATE].isOn;
        Data.UseDistance = m_Radios[RADIO_DISTANCE].isOn;
        Data.UseHot = m_Radios[RADIO_HOT].isOn;

        Data.UseRoad = m_Radios[RADIO_ROAD].isOn;
        Data.UseDistrict = m_Radios[RADIO_DISTRICT].isOn;
        Data.UsePlaza = m_Radios[RADIO_PLAZA].isOn;
    }

    public override void OnApplyClicked()
    {
        UI.Instance.Find<UIFilter>().Invalidate();

        base.OnApplyClicked();
    }

    public void OnRadioValueChanged(int id, bool enabled)
    {
        m_Radios[id].GetComponentInChildren<Image>().color = enabled ?
             m_ColorToogleEnabled :
             m_ColorToogleDisabled;
    }
}