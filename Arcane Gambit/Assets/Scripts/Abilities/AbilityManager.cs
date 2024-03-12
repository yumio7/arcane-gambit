using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private AbilityFactory _abilityFactory = new AbilityFactory();
    // Start is called before the first frame update
    void Start()
    {
        IAbility fartAbility = _abilityFactory.CreateNewAbility().AddInput().AddInput().Fart().Build();
        fartAbility.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
