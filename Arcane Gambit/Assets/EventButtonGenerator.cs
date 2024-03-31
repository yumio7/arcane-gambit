using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventButtonGenerator : MonoBehaviour
{
    private static EventButtonGenerator instance;
    // Singleton instance
    public static EventButtonGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("Event Button Manager is null!");
            }
            return instance;
        }
    }

    [Header("UI references")]
    public Canvas main_canavs;

    // Prefab for button to be created
    private GameObject button_prefab;

    // Struct for holding data
    public struct EventButtonData
    {
        public GameObject gameobject_reference;
        public EventTrigger trigger_reference;
    }

    private void Awake()
    {
        instance = this;
    }

    // Generates a UI Event button at a specific point on the canvas with a given text which
    // can be setupt to call any single method void argument. 
    public EventButtonData CreateNewEventButton(int xPos, int yPos, string text)
    {
        EventButtonData new_data = new EventButtonData();
        // Get button prefab
        if (button_prefab == null)
        {
            button_prefab = Resources.Load<GameObject>("BasicEventButton");
        }

        // Create the button and put it on the canvas
        GameObject new_button = Instantiate(button_prefab);
        new_button.transform.SetParent(main_canavs.transform);
        new_button.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

        // Set Button Text
        EventTriggerButton trigger_script = new_button.GetComponent<EventTriggerButton>();
        trigger_script.SetDisplayText(text);

        // Return struct with necessary info
        new_data.gameobject_reference = new_button;
        new_data.trigger_reference = trigger_script;
        return new_data;
    }

    // Overloaded version that also instantly sets up button with given method and argument
    public EventButtonData CreateNewEventButton(int xPos, int yPos, string text, Action<object> method, object argument)
    {
        EventButtonData new_data = CreateNewEventButton(xPos, yPos, text);
        new_data.trigger_reference.SetEventTriggerParameters(method, argument);
        return new_data;
    }
}
