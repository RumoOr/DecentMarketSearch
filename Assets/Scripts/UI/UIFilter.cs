using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIFilter : UISidePanel
{
    private const int TOGGLE_PRICE = 0;
    private const int TOGGLE_ROAD = 1;
    private const int TOGGLE_DISTRICT = 2;
    private const int TOGGLE_PLAZA = 3;
    private const int TOGGLE_HOT = 4;

    private const int SLIDER_PRICE_MIN = 0;
    private const int SLIDER_PRICE_MAX = 1;
    private const int SLIDER_ROAD_DIST_MAX = 2;
    private const int SLIDER_DISTRICT_DIST_MAX = 3;
    private const int SLIDER_PLAZA_DIST_MAX = 4;

    private const int PROXIMITY_DISTANCE_MAX = 9;

    public const int PRICE_MAXIMUM = 100000;

    public Color m_ColorToogleEnabled;
    public Color m_ColorToogleDisabled;

    private Toggle[] m_Toggles;

    private Slider[] m_Slider;

    private Text[] m_SliderTexts;

    public ParcelFilter Data { get; private set; }

    public override void OnCreate()
    {
        base.OnCreate();

        var index = 0;
        m_Toggles = GetComponentsInChildren<Toggle>(true);
        foreach (var toggle in m_Toggles)
        {
            var id = index;
            index++;

            toggle.onValueChanged.AddListener((bool enabled) =>
                OnToggleValueChanged(id, enabled));
            toggle.isOn = false;
        }

        index = 0;
        m_Slider = GetComponentsInChildren<Slider>(true);
        foreach (var slider in m_Slider)
        {
            var id = index;
            index++;

            slider.onValueChanged.AddListener((float value) =>
                OnSliderValueChanged(id, value));
        }

        m_SliderTexts = m_Slider
            .Select(s => s.GetComponentsInChildren<Text>()[1])
            .ToArray();

        Data = new ParcelFilter();

        IsExpanded = true;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        Data.UsePrice = m_Toggles[TOGGLE_PRICE].isOn;
        Data.UseRoad = m_Toggles[TOGGLE_ROAD].isOn;
        Data.UseDistrict = m_Toggles[TOGGLE_DISTRICT].isOn;
        Data.UsePlaza = m_Toggles[TOGGLE_PLAZA].isOn;
        Data.UseHot = m_Toggles[TOGGLE_HOT].isOn;

        Data.PriceMinValue = (int)m_Slider[SLIDER_PRICE_MIN].value;
        Data.PriceMaxValue = (int)m_Slider[SLIDER_PRICE_MAX].value;

        Data.RoadMaxDist = (int)m_Slider[SLIDER_ROAD_DIST_MAX].value;
        Data.DistrictMaxDist = (int)m_Slider[SLIDER_DISTRICT_DIST_MAX].value;
        Data.PlazaMaxDist = (int)m_Slider[SLIDER_PLAZA_DIST_MAX].value;
    }

    public override void OnApplyClicked()
    {
        UI.Instance.Find<UIOrder>().Invalidate();

        base.OnApplyClicked();
    }

    public void OnToggleValueChanged(int id, bool enabled)
    {
        m_Toggles[id].GetComponentInChildren<Image>().color = enabled ?
            m_ColorToogleEnabled :
            m_ColorToogleDisabled;
    }

    public void OnSliderValueChanged(int id, float value)
    {
        m_SliderTexts[id].text = string.Format("{0:N0}", value);

        if (id == SLIDER_PRICE_MIN &&
            value > m_Slider[SLIDER_PRICE_MAX].value)
            m_Slider[SLIDER_PRICE_MAX].value = value;
        else if (id == SLIDER_PRICE_MAX &&
            value < m_Slider[SLIDER_PRICE_MIN].value)
            m_Slider[SLIDER_PRICE_MIN].value = value;
    }

    public void DoUpdate()
    {
        if (DataManager.Instance.Count == 0)
            return;

        var minPrice = DataManager.Instance.Parcels
            .Where(p => p.publication.price > 0)
            .Min(p => p.publication.price);

        var maxPrice = Mathf.Clamp(
            DataManager.Instance.Parcels.Max(p => p.publication.price),
            0, PRICE_MAXIMUM);

        m_Slider[SLIDER_PRICE_MIN].minValue = minPrice;
        m_Slider[SLIDER_PRICE_MIN].maxValue = maxPrice;
        m_Slider[SLIDER_PRICE_MIN].value = m_Slider[SLIDER_PRICE_MIN].minValue;

        m_Slider[SLIDER_PRICE_MAX].minValue = minPrice;
        m_Slider[SLIDER_PRICE_MAX].maxValue = maxPrice;
        m_Slider[SLIDER_PRICE_MAX].value = m_Slider[SLIDER_PRICE_MAX].maxValue;

        m_Slider[SLIDER_ROAD_DIST_MAX].minValue = 0;
        m_Slider[SLIDER_ROAD_DIST_MAX].maxValue = PROXIMITY_DISTANCE_MAX;
        m_Slider[SLIDER_DISTRICT_DIST_MAX].minValue = 0;
        m_Slider[SLIDER_DISTRICT_DIST_MAX].maxValue = PROXIMITY_DISTANCE_MAX;
        m_Slider[SLIDER_PLAZA_DIST_MAX].minValue = 0;
        m_Slider[SLIDER_PLAZA_DIST_MAX].maxValue = PROXIMITY_DISTANCE_MAX;

        for (var i = 0; i < m_Slider.Length; i++)
            OnSliderValueChanged(i, m_Slider[i].value);

        Invalidate();
    }
}
