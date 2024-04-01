using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTriggerButton : EventTrigger
{
    [Header("UI References")]
    public Text display_text;
    public void SetDisplayText(string text)
    {
        if(display_text != null)
        {
            display_text.text = text;
        }
    }
}
