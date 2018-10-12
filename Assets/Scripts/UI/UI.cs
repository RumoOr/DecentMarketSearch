using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    private static UI s_Instance;

    private UIComponent[] m_UIComponents;

    public static UI Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<UI>();
                s_Instance.OnCreate();
            }
            return s_Instance;
        }
    }

    public void OnCreate()
    {
        m_UIComponents = GetComponentsInChildren<UIComponent>(true);

        foreach (var c in m_UIComponents)
            c.OnCreate();
    }

    public T Find<T>() where T : UIComponent
    {
        foreach (var c in m_UIComponents)
        {
            if (c is T) return (T)c;
        }
        return null;
    }
}
