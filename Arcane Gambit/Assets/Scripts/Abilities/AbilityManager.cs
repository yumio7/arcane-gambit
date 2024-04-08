using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private AbilityFactory _abilityFactory = new AbilityFactory();

    public Dictionary<string, IAbility> AbilityDirectory { get; private set; } = new Dictionary<string, IAbility>();

    private List<EventButtonGenerator.EventButtonData> _abilityDisplay =
        new List<EventButtonGenerator.EventButtonData>();
    
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
    
    private void Start()
    {
        InitializeAbilityDirectory();
        HideAbilityDirectory();
    }

    // Update is called once per frame
    void Update()
    {
        #region TEMP_INPUT
        
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            DisplayAbilityDirectory();
        }

        #endregion
        
    }

    private void InitializeAbilityDirectory()
    {
        AbilityDirectory.Add("Modify Single Card", 
            new ModifyCardAbility(new ConcreteAbilityInput<Card>(null,() => Poker.Instance.Players[0].Hand.GetCard(0)), 
                new AbilityInput<ModifyKey>(ModifyKey.IncrementRank, 
                    new ButtonInputRequest<ModifyKey>(new List<ModifyKey>() 
                    {ModifyKey.IncrementRank, 
                        ModifyKey.DecrementRank,
                        ModifyKey.Club,
                        ModifyKey.Diamond,
                        ModifyKey.Heart,
                        ModifyKey.Spade
                    }))));
        AbilityDirectory.Add("The Magician", 
            new ModifyCardAbility(new ConcreteAbilityInput<Card>(null,() => Poker.Instance.Players[0].Hand.GetCard(0)), 
                new AbilityInput<ModifyKey>(ModifyKey.IncrementRank, 
                    new ButtonInputRequest<ModifyKey>(new List<ModifyKey>() 
                    {ModifyKey.IncrementRank, 
                        ModifyKey.DecrementRank
                    }))));
        /*AbilityDirectory.Add("Swap Two Cards",
            new SwapCardAbility(new ConcreteAbilityInput<Card>(Poker.Instance.Players[0].Hand.Cards[0]), 
                new ConcreteAbilityInput<Card>(Poker.Instance.Players[1].Hand.Cards[0])));
        AbilityDirectory.Add("Swap Hands",
            new SwapHandAbility(new ConcreteAbilityInput<Hand>(Poker.Instance.Players[0].Hand), 
                new ConcreteAbilityInput<Hand>(Poker.Instance.Players[1].Hand)));*/
        AbilityDirectory.Add("The Fool", 
            new ModifyCardAbility(new ConcreteAbilityInput<Card>(null,() => Poker.Instance.Players[0].Hand.GetCard(0),
                    new ConcreteAbilityInput<Card>(null,() => Poker.Instance.Players[0].Hand.GetCard(1),
                        new ConcreteAbilityInput<Card>(null,() => Poker.Instance.Players[0].Hand.GetCard(2),
                            new ConcreteAbilityInput<Card>(null,() => Poker.Instance.Players[0].Hand.GetCard(3),
                                new ConcreteAbilityInput<Card>(null,() => Poker.Instance.Players[0].Hand.GetCard(4)))))), 
                new ConcreteAbilityInput<ModifyKey>(ModifyKey.NewCardFromDeck, () => ModifyKey.NewCardFromDeck)));
        
        
        EventButtonGenerator EBG = EventButtonGenerator.Instance;
        foreach (KeyValuePair<string,IAbility> ability in AbilityDirectory)
        {
            EventButtonGenerator.EventButtonData data = EBG.CreateNewEventButton(0, 0, ability.Key, o => ActivateAbility(ability.Value), null);
            EBG.PlaceButtonInLayout(data);
            _abilityDisplay.Add(data);
        }
    }

    private void DisplayAbilityDirectory()
    {
        foreach (EventButtonGenerator.EventButtonData buttonData in _abilityDisplay)
        {
            buttonData.gameobject_reference.SetActive(true);
        }
    }

    private void HideAbilityDirectory()
    {
        foreach (EventButtonGenerator.EventButtonData buttonData in _abilityDisplay)
        {
            buttonData.gameobject_reference.SetActive(false);
        }
    }

    private int counter = 0;
    public void ActivateAbility(IAbility ability)
    {
        Debug.Log($"Debug: {counter++}");
        _abilityControlLoopCoroutine = StartCoroutine(AbilityControlLoopCoroutine(ability));
        HideAbilityDirectory();
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
}