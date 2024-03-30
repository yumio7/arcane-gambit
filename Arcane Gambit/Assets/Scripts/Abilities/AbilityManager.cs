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
            //IAbility thing = new SwapCardAbility(_poker.Players[0].Hand.Cards[0], _poker.Players[1].Hand.Cards[0]);
            //thing.Activate(this);
            //IAbility thing = new SwapCardAbility(new ConcreteAbilityInput<Card>(_poker.Players[0].Hand.Cards[0]), new AbilityInput<Card>());
            IAbility thing2 = new SwapCardAbility(new ConcreteAbilityInput<Card>(_poker.Players[0].Hand.Cards[0]), new ConcreteAbilityInput<Card>(_poker.Players[1].Hand.Cards[0]));
            ActivateAbility(thing2);
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            IAbility thing1 = new ModifyCardAbility(
                new ConcreteAbilityInput<Card>(_poker.Players[0].Hand.Cards[0],
                    new ConcreteAbilityInput<Card>(_poker.Players[1].Hand.Cards[0])),
                new ConcreteAbilityInput<ModifyKey>(ModifyKey.IncrementRank, 
                    new ConcreteAbilityInput<ModifyKey>(ModifyKey.Club)));
            ActivateAbility(thing1);
        }
    }
    
    public void ActivateAbility(IAbility ability)
    {
        ability.Activate(this);
    }
}
