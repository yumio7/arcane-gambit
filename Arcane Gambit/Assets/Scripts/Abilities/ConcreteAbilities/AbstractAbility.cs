using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAbility : IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; }

    public void Setup()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Activate()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Process()
    {
        yield break;
    }

    public virtual void Finish()
    {
        NextAbility?.Activate();
    }

    public void Cleanup()
    {
        throw new System.NotImplementedException();
    }

    public virtual void RequestInput()
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
