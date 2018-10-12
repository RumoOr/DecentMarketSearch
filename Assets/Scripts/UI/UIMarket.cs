using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMarket : UIComponent
{
    private ScrollRect m_ScrollRect;

    private UIAuctions m_UIAuctions;

    private Action<UIParcel> m_OnParcelClickListener;

    private Action m_OnRefreshListener;

    public int Count { get { return m_UIAuctions != null ? m_UIAuctions.Count : 0; } }

    public override void OnCreate()
    {
        base.OnCreate();

        m_ScrollRect = GetComponentInChildren<ScrollRect>(true);

        m_UIAuctions = GetComponentInChildren<UIAuctions>(true);
    }

    public void OnEnable()
    {
        ResetScroll();
    }

    public void OnParcelClicked(UIParcel parcel)
    {
        if (m_OnParcelClickListener != null)
            m_OnParcelClickListener.Invoke(parcel);
    }

    public void OnRefreshClicked()
    {
        if (m_OnRefreshListener != null)
            m_OnRefreshListener.Invoke();
    }

    public void OnPageChanged(int page, int pageCount)
    {
        ResetScroll();
    }

    public void ResetScroll()
    {
        StartCoroutine(ResetScrollRoutine());
    }

    public void DoUpdate(Parcel[] data, Action<UIParcel> onParcelClickListener = null,
        Action onRefreshListener = null)
    {
        m_OnParcelClickListener = onParcelClickListener;

        m_OnRefreshListener = onRefreshListener;       

        m_UIAuctions.DoUpdate(data, OnParcelClicked, OnPageChanged, OnRefreshClicked);
    }

    public void SetRefreshButtonVisibility(bool isVisible)
    {
        m_UIAuctions.SetRefreshButtonVisibility(isVisible);
    }

    private IEnumerator ResetScrollRoutine()
    {
        yield return new WaitForFixedUpdate();

        if (m_ScrollRect != null)
            m_ScrollRect.verticalNormalizedPosition = 1;
    }
}
