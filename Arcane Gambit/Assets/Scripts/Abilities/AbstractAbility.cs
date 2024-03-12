using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAbility : IAbility
{
    public IAbility NextCard { get; set; }
    public string AbilityName { get; set; }
    public virtual void Activate()
    {
        this.Process();
        NextCard?.Activate();
    }

    public virtual void Process()
    {
        
    }

    public virtual void AddInput()
    {
        throw new System.NotImplementedException();
    }
}
