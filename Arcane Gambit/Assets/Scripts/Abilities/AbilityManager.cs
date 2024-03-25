using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Poker _poker;
    private AbilityFactory _abilityFactory = new AbilityFactory();
    // Start is called before the first frame update
    void Start()
    {
        //IAbility fartAbility = _abilityFactory.CreateNewAbility().AddInput().AddInput().Fart().Build();
        //fartAbility.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            IAbility thing = new SwapCardAbility(_poker.Players[0].Hand.Cards[0], _poker.Players[1].Hand.Cards[0]);
            thing.Activate(this);
        }
    }
}
