using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerTest : MonoBehaviour
{
    void Start()
    {
        EventButtonGenerator.EventButtonData data1 = EventButtonGenerator.Instance.CreateNewEventButton(0, 0, "button 1");
        EventButtonGenerator.EventButtonData data2 = EventButtonGenerator.Instance.CreateNewEventButton(-200, 200, "button 2");
        EventButtonGenerator.EventButtonData data3 = EventButtonGenerator.Instance.CreateNewEventButton(200, -200, "button 3");

        data1.trigger_reference.SetEventTriggerParameters(Method1, 5);
        data2.trigger_reference.SetEventTriggerParameters(Method1, 10);
        data3.trigger_reference.SetEventTriggerParameters(Method1, 15);
    }

    private void Method1(object value)
    {
        Debug.Log("Value: " + value.ToString());
    }

    private int Method2(object value)
    {
        return (int)value;
    }
}
