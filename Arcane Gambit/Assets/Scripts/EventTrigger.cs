using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventTrigger : MonoBehaviour
{
    private Action<object> method;
    private object argument;

    // Method for passing in event trigger parameters
    public void SetEventTriggerParameters(Action<object> method, object argument)
    {
        this.method = method;
        this.argument = argument;
    }
    // Method that triggers given event
    public void TriggerEvent()
    {
        if (method != null)
        {
            method?.Invoke(argument);
        } else
        {
            Debug.LogWarning("Event Trigger has no method!");
        }
    }
}
