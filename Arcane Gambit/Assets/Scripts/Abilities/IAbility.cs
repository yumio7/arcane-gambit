using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; }
    
    public void Activate(AbilityManager abilityManager);
    public IEnumerator Process(AbilityManager abilityManager);
    public void Finish(AbilityManager abilityManager);
    public void RequestInput();
}
