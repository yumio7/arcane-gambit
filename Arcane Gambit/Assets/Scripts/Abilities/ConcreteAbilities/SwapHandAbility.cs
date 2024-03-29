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
    
    public void Activate(AbilityManager abilityManager)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Process(AbilityManager abilityManager)
    {
        throw new System.NotImplementedException();
    }

    public void Finish(AbilityManager abilityManager)
    {
        throw new System.NotImplementedException();
    }

    public void RequestInput()
    {
        throw new System.NotImplementedException();
    }
}
