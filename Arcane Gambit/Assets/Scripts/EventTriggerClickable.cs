using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerClickable : EventTrigger, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool is_clickable = true;
    private bool mouse_over = false;

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

    public bool IsMouseOver()
    {
        return mouse_over;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }
}
