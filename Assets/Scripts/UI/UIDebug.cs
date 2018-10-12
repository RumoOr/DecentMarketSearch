using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : UIComponent
{
    private const int DEFAULT_HISTORY_LENGTH = 3;

    public int m_HistoryLength = DEFAULT_HISTORY_LENGTH;

    private Text m_TextMessage;

    private readonly Queue<string> m_History = new Queue<string>();
    
    private int Count { get { return m_History.Count; } }

    public override void OnCreate()
    {
        base.OnCreate();

        m_TextMessage = GetComponentInChildren<Text>(true);
    }

    public void Log(string text)
    {
        m_History.Enqueue(text);

        if (Count > m_HistoryLength)
            m_History.Dequeue();

        m_TextMessage.text = m_History
            .Reverse()
            .Aggregate((c, n) => c + "\n" + n);

        Debug.Log(text);
    }
}
