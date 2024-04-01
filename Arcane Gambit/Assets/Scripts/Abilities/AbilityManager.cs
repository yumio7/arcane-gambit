using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private AbilityFactory _abilityFactory = new AbilityFactory();

    private Coroutine _abilityControlLoopCoroutine;
    // Making it a Singleton.
    private static AbilityManager _instance;
    
    
    public static AbilityManager Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AbilityManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(AbilityManager).ToString();
                    _instance = go.AddComponent<AbilityManager>();
                }
            }
            return _instance;
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        //IAbility fartAbility = _abilityFactory.CreateNewAbility().AddInput().AddInput().Fart().Build();
        //fartAbility.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        #region TEMP_INPUT

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            //IAbility thing = new SwapCardAbility(_poker.Players[0].Hand.Cards[0], _poker.Players[1].Hand.Cards[0]);
            //thing.Activate(this);
            //IAbility thing = new SwapCardAbility(new ConcreteAbilityInput<Card>(_poker.Players[0].Hand.Cards[0]), new AbilityInput<Card>());
            IAbility thing2 = new SwapCardAbility(new ConcreteAbilityInput<Card>(Poker.Instance.Players[0].Hand.Cards[0]), new ConcreteAbilityInput<Card>(Poker.Instance.Players[1].Hand.Cards[0]));
            ActivateAbility(thing2);
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            IAbility thing1 = new ModifyCardAbility(
                new ConcreteAbilityInput<Card>(Poker.Instance.Players[0].Hand.Cards[0],
                    new ConcreteAbilityInput<Card>(Poker.Instance.Players[1].Hand.Cards[0])),
                new ConcreteAbilityInput<ModifyKey>(ModifyKey.IncrementRank, 
                    new ConcreteAbilityInput<ModifyKey>(ModifyKey.Club)));
            ActivateAbility(thing1);
        }
        
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            IAbility thing3 = new ModifyCardAbility(new ConcreteAbilityInput<Card>(Poker.Instance.Players[0].Hand.Cards[0], new ConcreteAbilityInput<Card>(Poker.Instance.Players[1].Hand.Cards[0])), 
                new AbilityInput<ModifyKey>(ModifyKey.IncrementRank, new ButtonInputRequest<ModifyKey>(new List<ModifyKey>() {ModifyKey.IncrementRank, ModifyKey.DecrementRank})));

            ActivateAbility(thing3);
        }

        #endregion
        
    }

    private int counter = 0;
    public void ActivateAbility(IAbility ability)
    {
        Debug.Log($"Debug: {counter++}");
        _abilityControlLoopCoroutine = StartCoroutine(AbilityControlLoopCoroutine(ability));
    }

    private IEnumerator AbilityControlLoopCoroutine(IAbility ability)
    {
        ability.Setup();
        Debug.Log($"Debug: {counter++}");
        ability.RequestInput();
        Debug.Log($"Debug: {counter++}");

        while (true)
        {
            if (ability.IsReady())
            {
                Debug.Log($"Debug: {counter++}");

                ability.Activate();
                break;
            }
            yield return null;
        }

        while (!ability.IsFinished())
        {
            yield return null;
        }
        ability.Cleanup();
        Debug.Log($"Debug: {counter++}");

        Debug.Log("Finished ability");
    }

    private void OnDisable()
    {
        
    }
}