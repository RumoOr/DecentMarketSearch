using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : UIComponent
{
    private Text m_TextPage;

    private Image m_ImageBar;

    public Image m_ImageFooterBar;

    public override void OnCreate()
    {
        base.OnCreate();

        m_TextPage = GetComponentsInChildren<Text>()[1];

        m_ImageBar = GetComponentsInChildren<Image>()
            .First(i => i.name == "Progress");
    }

    public void DoUpdate(int index, int count, string prefix = "Page")
    {
        m_TextPage.text = string.Format("{2} {0} of {1}", index, count, prefix);

        m_ImageBar.fillAmount = (float)index / count;

        m_ImageFooterBar.fillAmount = index == count ? 0 : m_ImageBar.fillAmount;
    }
}
