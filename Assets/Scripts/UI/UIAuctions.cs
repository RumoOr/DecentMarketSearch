using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAuctions : UIComponent
{
    private const int BUTTON_ARROW_DOUBLE_LEFT = 0;
    private const int BUTTON_ARROW_LEFT = 1;
    private const int BUTTON_ARROW_RIGHT = 2;
    private const int BUTTON_ARROW_DOUBLE_RIGHT = 3;
    private const int BUTTON_REFRESH = 4;

    private GridLayoutGroup m_GridView;

    private Button[] m_Buttons;

    private Text m_TextPage;

    private UIParcel[] m_UIParcels;

    private Action<UIParcel> m_OnParcelClickListener;

    private Action<int, int> m_OnPageChangeListener;

    private Action m_OnRefreshListener;

    public UIParcel this[int index]
    {
        get { return m_UIParcels != null ? m_UIParcels[index] : null; }
    }

    private Parcel[] m_Data;

    private int m_CurrentPage;

    public int Count { get { return m_UIParcels != null ? m_UIParcels.Length : 0; } }

    public int PageCount
    {
        get
        {
            return m_Data != null ?
               (m_Data.Length / Count) +
                    (m_Data.Length % Count == 0 ? 0 : 1) :
                0;
        }
    }

    public override void OnCreate()
    {
        base.OnCreate();

        m_GridView = GetComponentInChildren<GridLayoutGroup>(true);

        m_UIParcels = m_GridView.GetComponentsInChildren<UIParcel>(true);

        var footer = GetComponentsInChildren<HorizontalLayoutGroup>(true)
            .First(g => g.transform.parent.name == "Footer").transform;

        var buttons = new List<Button>();
        for (var i = 1; i < transform.childCount; i++)
            buttons.AddRange(transform.GetChild(i).GetComponentsInChildren<Button>(true));

        m_Buttons = buttons.ToArray();
        for (var i = 0; i < m_Buttons.Length; i++)
        {
            var id = i;
            m_Buttons[i].onClick.AddListener(() => OnButtonClicked(id));
        }

        m_TextPage = footer.GetComponentInChildren<Text>(true);
    }

    public void OnButtonClicked(int id)
    {
        switch (id)
        {
            case BUTTON_ARROW_DOUBLE_LEFT:
                SetCurrentPage(0);
                break;
            case BUTTON_ARROW_LEFT:
                SetCurrentPage(m_CurrentPage - 1);
                break;
            case BUTTON_ARROW_DOUBLE_RIGHT:
                SetCurrentPage(PageCount - 1);
                break;
            case BUTTON_ARROW_RIGHT:
                SetCurrentPage(m_CurrentPage + 1);
                break;
            case BUTTON_REFRESH:
                if (m_OnRefreshListener != null)
                    m_OnRefreshListener.Invoke();
                break;
        }
    }

    public void OnParcelClicked(UIParcel parcel)
    {
        if (m_OnParcelClickListener != null)
            m_OnParcelClickListener.Invoke(parcel);
    }

    public void OnMapLoaded(Parcel parcel, Texture texture)
    {
        var uiParcel = m_UIParcels.FirstOrDefault(p => p.Data.id == parcel.id);
        if (uiParcel)
            uiParcel.SetMap((Texture2D)texture);
    }

    public void DoUpdate(Parcel[] data, Action<UIParcel> onParcelClickListener = null,
        Action<int, int> onPageChangeListener = null, Action onRefreshListener = null)
    {
        m_OnParcelClickListener = onParcelClickListener;

        m_OnPageChangeListener = onPageChangeListener;

        m_OnRefreshListener = onRefreshListener;

        m_Data = data;

        SetCurrentPage(0);
    }

    public void SetRefreshButtonVisibility(bool isVisible)
    {
        m_Buttons[BUTTON_REFRESH].gameObject.SetActive(isVisible);
    }

    private void SetCurrentPage(int page)
    {
        m_CurrentPage = page;

        if (m_Data != null)
        {
            var pageData = m_Data
                .Skip(page * Count)
                .Take(Count)
                .ToArray();

            for (var i = 0; i < Count; i++)
            {
                if (pageData.Length > i)
                {
                    this[i].gameObject.SetActive(true);
                    this[i].DoUpdate(pageData[i], OnParcelClicked);

                    DataManager.Instance.GetMap(this, pageData[i], OnMapLoaded);
                }
                else
                    this[i].gameObject.SetActive(false);
            }
        }

        m_TextPage.text = string.Format(
            page < PageCount - 1 ? "{0} <color=#8F9199FF>... {1}</color>" : "{0}",
            page + 1, PageCount);

        m_Buttons[BUTTON_ARROW_DOUBLE_LEFT].gameObject.SetActive(page > 0);
        m_Buttons[BUTTON_ARROW_LEFT].gameObject.SetActive(page > 0);

        m_Buttons[BUTTON_ARROW_RIGHT].gameObject.SetActive(page < PageCount - 1);
        m_Buttons[BUTTON_ARROW_DOUBLE_RIGHT].gameObject.SetActive(page < PageCount - 1);

        if (m_OnPageChangeListener != null)
            m_OnPageChangeListener.Invoke(page, PageCount);
    }
}
