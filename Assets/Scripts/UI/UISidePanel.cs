using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISidePanel : UIComponent
{
    private const float LAYOUT_WIDTH_MIN = 1;
    private const float LAYOUT_WIDTH_MAX = 5;

    public Action<bool> OnExpandChangeEvent;

    public Action OnApplyClickEvent;

    private LayoutElement m_Layout;

    private Transform m_ContentGroup;

    private Image m_ImageExpand;

    private bool m_IsExpaned;

    public bool IsExpanded
    {
        get { return m_IsExpaned; }
        set { m_IsExpaned = value; OnExpandedChanged(); }
    }

    public override void OnCreate()
    {
        base.OnCreate();

        m_Layout = GetComponent<LayoutElement>();

        m_ContentGroup = transform.GetChild(0).GetChild(1);

        var buttonExpand = GetComponentInChildren<Button>();
        buttonExpand.onClick.AddListener(() => IsExpanded = !IsExpanded);

        m_ImageExpand = buttonExpand.GetComponentInChildren<Image>(true);

        var buttonApply = GetComponentsInChildren<Button>().Last();
        buttonApply.onClick.AddListener(OnApplyClicked);
    }

    public void OnExpandedChanged()
    {
        m_ImageExpand.transform.localScale = IsExpanded ?
            Vector3.one :
            new Vector3(-1, 1, 1);

        m_Layout.flexibleWidth = IsExpanded ?
            LAYOUT_WIDTH_MAX : LAYOUT_WIDTH_MIN;

        m_ContentGroup.gameObject.SetActive(IsExpanded);

        Invalidate();

        if (OnExpandChangeEvent != null)
            OnExpandChangeEvent.Invoke(IsExpanded);
    }

    public virtual void OnApplyClicked()
    {
        Invalidate();

        if (OnApplyClickEvent != null)
            OnApplyClickEvent.Invoke();
    }
}
