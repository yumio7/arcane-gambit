using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFactory
{
    private IAbility _ability;

    public AbilityFactory CreateNewAbility(string abilityName = "")
    {
        _ability = new BasicAbility();
        _ability.AbilityName = abilityName;
        return this;
    }
        
    public IAbility Build()
    {
        return _ability;
    }

    public AbilityFactory AddInput()
    {
        this._ability.RequestInput();
        return this;
    }

    public AbilityFactory Fart()
    {
        return this;
    }
}
