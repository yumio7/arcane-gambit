using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IAbilityInputRequest<T>
{
    public void Request(Action<T> setMethod);
    public void Cleanup();
}

public abstract class AbstractInputRequest<T> : IAbilityInputRequest<T>
{
    //protected 
    
    public void Request(Action<T> setMethod)
    {
        throw new NotImplementedException();
    }

    public void Cleanup()
    {
        throw new NotImplementedException();
    }
}

public class KeyPressInputRequest<T> : IAbilityInputRequest<T>
{
    public void Request(Action<T> method)
    {
        // Implement the functionality here.
    }

    public void Cleanup()
    {
        throw new NotImplementedException();
    }
}

public class ButtonInputRequest<T> : IAbilityInputRequest<T>
{
    private List<T> _items = new List<T>();
    private List<EventButtonGenerator.EventButtonData> _buttonDatas = new List<EventButtonGenerator.EventButtonData>();
    //private Action<T> _setMethod;
    
    public ButtonInputRequest(List<T> items)
    {
        _items = items;
    }
    public void Request(Action<T> setMethod)
    {
        
        int temp = 0;
        foreach (T item in _items)
        {
            var itemObject = item;
            Action<object> setMethodObject = _ => setMethod(itemObject);
            EventButtonGenerator.EventButtonData buttonData =
                EventButtonGenerator.Instance.CreateNewEventButton(0, temp, item.ToString(), setMethodObject, item);
            Debug.Log(buttonData);
            _buttonDatas.Add(buttonData);
            Debug.Log("test");
            temp += 120;
        }
    }

    public void Cleanup()
    {
        foreach (var buttonData in _buttonDatas)
        {
            MonoBehaviour.Destroy(buttonData.gameobject_reference);
        }
        _buttonDatas.Clear();
    }
}