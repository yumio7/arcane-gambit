using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityInputRequest<T>
{
    public void Request(Action<T> setMethod);
}

public class KeyPressInputRequest<T> : IAbilityInputRequest<T>
{
    public void Request(Action<T> method)
    {
        // Implement the functionality here.
    }
}