using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility : IDisposable
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; }
    
    
    public void Activate();
    public IEnumerator Process();
    public void Finish();
    public void Cleanup();
    public void RequestInput();
    public bool IsReady();
    public bool IsFinished();
}
