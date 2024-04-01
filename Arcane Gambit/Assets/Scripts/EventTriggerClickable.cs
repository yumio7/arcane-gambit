using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerClickable : EventTrigger, IPointerClickHandler
{
    private bool is_clickable = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (is_clickable)
        {
            TriggerEvent();
        }
    }

    public bool IsClickable()
    {
        return is_clickable;
    }

    public void SetIsClickable(bool clickable)
    {
        is_clickable = clickable;
    }
}
