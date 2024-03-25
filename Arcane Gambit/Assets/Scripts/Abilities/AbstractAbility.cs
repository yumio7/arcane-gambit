using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAbility : IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; }

    public virtual void Activate(AbilityManager abilityManager)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Process(AbilityManager abilityManager)
    {
        yield break;
    }

    public virtual void Finish(AbilityManager abilityManager)
    {
        NextAbility?.Activate(abilityManager);
    }

    public virtual void RequestInput()
    {
        throw new System.NotImplementedException();
    }
}
