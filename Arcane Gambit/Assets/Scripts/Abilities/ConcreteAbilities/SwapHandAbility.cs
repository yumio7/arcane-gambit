using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapHandAbility : IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; } = "Swap Hands";
    
    private IAbilityInput<Hand> _input;
    
    public SwapHandAbility(IAbilityInput<Hand> input1, IAbilityInput<Hand> input2)
    {
        _input = input1;
        _input.LinkSequence(input2);
    }

    public void Setup()
    {
        throw new System.NotImplementedException();
    }

    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Process()
    {
        throw new System.NotImplementedException();
    }

    public void Finish()
    {
        throw new System.NotImplementedException();
    }

    public void Cleanup()
    {
        throw new System.NotImplementedException();
    }

    public void RequestInput()
    {
        throw new System.NotImplementedException();
    }

    public bool IsReady()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFinished()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
