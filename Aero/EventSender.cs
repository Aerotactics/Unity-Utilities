using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EventSender : MonoBehaviour
{
    [SerializeField] private string m_eventName = string.Empty;

    public void Start()
    {
        GetComponent<Button>()?.onClick.AddListener(Send);
    }

    public void Send()
    {
        Aero.Events.Invoke(m_eventName);
    }
}
