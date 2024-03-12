using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    public IAbility NextCard { get; set; }
    public string AbilityName { get; set; }
    
    public void Activate();
    public void Process();
    public void AddInput();
}
