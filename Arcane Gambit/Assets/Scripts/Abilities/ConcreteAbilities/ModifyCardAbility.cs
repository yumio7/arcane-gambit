using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifyKey
{
    IncrementRank,
    DecrementRank,
    Spade,
    Club,
    Heart,
    Diamond
}

public class ModifyCardAbility : IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; }
    
    private IAbilityInput<Card> _input;
    private IAbilityInput<ModifyKey> _modify;
    private readonly Action _onInputReceived;
    public bool Finished { get; private set; } = false;

    public ModifyCardAbility(IAbilityInput<Card> cardToModify, IAbilityInput<ModifyKey> modification)
    {
        // Check if the inputs are null
        if (cardToModify == null || modification == null)
        {
            throw new ArgumentNullException(nameof(cardToModify), "The provided arguments must not be null.");
        }

        _input = cardToModify;
        _modify = modification;

        _onInputReceived = () => RequestInput();
    }

    /*public ModifyCardAbility(IAbilityInput<Card> cardToModify, IAbilityInput<ModifyKey> modification)
    {
        // Check if the inputs are null
        if (cardToModify == null || modification == null)
        {
            throw new ArgumentNullException(nameof(cardToModify), "The provided arguments must not be null.");
        }

        _input = cardToModify;
        _modify = modification;

        _onInputReceived = () => RequestInput();
    }*/
    
    public void Dispose()
    {
        Debug.Log("DISPOSE");
        _input.UnsubscribeFromSequence(_onInputReceived);
        _modify.UnsubscribeFromSequence(_onInputReceived);
    }

    public void Setup()
    {
        _input.SubscribeToSequence(_onInputReceived);
        _modify.SubscribeToSequence(_onInputReceived);
    }

    public void Activate()
    {
        AbilityManager.Instance.StartCoroutine(this.Process());
    }

    public IEnumerator Process()
    {
        foreach (Card card in _input)
        {
            CardCollection cardOwner = card.Owner;
            Card returnCard = card;
            Debug.Log(cardOwner);
            cardOwner.Remove(card);
            foreach (ModifyKey modify in _modify)
            {
                returnCard = ModifyCard(returnCard, modify); 
            }
            cardOwner.AddCard(returnCard);
        }
        yield return null;
        Finish();
    }

    private Card ModifyCard(Card card, ModifyKey modifyKey)
    {
        switch(modifyKey)
        {
            case ModifyKey.IncrementRank:
                return card + 1;
            case ModifyKey.DecrementRank:
                return card - 1;
            case ModifyKey.Spade:
                return card.SetSuit(SuitType.Spade);
            case ModifyKey.Club:
                return card.SetSuit(SuitType.Club);
            case ModifyKey.Heart:
                return card.SetSuit(SuitType.Heart);
            case ModifyKey.Diamond:
                return card.SetSuit(SuitType.Diamond);
            default:
                throw new ArgumentOutOfRangeException(nameof(modifyKey), modifyKey, null);
        } 
    }

    public void Finish()
    {
        Finished = true;
        NextAbility?.Activate();
    }

    public void Cleanup()
    {
        Dispose();
        _input.Cleanup();
        _modify.Cleanup();
        Finished = false;
    }

    public void RequestInput()
    {
        Debug.Log("request");
        if (_input?.IsInputSequenceReady() == false)
        {
            _input.RequestNextNonReadyInput();
        } else if (_modify?.IsInputSequenceReady() == false)
        {
            _modify.RequestNextNonReadyInput();
        }
    }

    public bool IsReady()
    {
        return _input.IsInputSequenceReady() && _modify.IsInputSequenceReady();
    }

    public bool IsFinished()
    {
        if (NextAbility != null)
        {
            return Finished || NextAbility.IsFinished();
        }
        return Finished;
    }
}
