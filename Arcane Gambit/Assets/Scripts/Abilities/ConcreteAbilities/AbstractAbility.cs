using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAbility : IAbility
{
    public bool Finished { get; protected set; } = false;

    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; }
    protected Action OnInputReceived;

    public abstract void Setup();

    public void Activate()
    {
        AbilityManager.Instance.StartCoroutine(this.Process());
    }

    public abstract IEnumerator Process();

    public void Finish()
    {
        Finished = true;
        NextAbility?.Activate();
    }

    public void Cleanup()
    {
        Dispose();
    }

    public abstract void RequestInput();

    public abstract bool IsReady();

    public bool IsFinished()
    {
        if (NextAbility != null)
        {
            return Finished || NextAbility.IsFinished();
        }
        return Finished;
    }

    public abstract void Dispose();
}
