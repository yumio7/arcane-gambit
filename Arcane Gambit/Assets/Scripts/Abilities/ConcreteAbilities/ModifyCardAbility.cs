using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyCardAbility : IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; }
    
    private IAbilityInput<Hand> _input;
    
    public ModifyCardAbility(IAbilityInput<Hand> input1)
    {
        _input = input1;
        //_input.LinkSequence(input2);
    }
    
    public void Activate(AbilityManager abilityManager)
    {
        abilityManager.StartCoroutine(this.Process(abilityManager));
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
