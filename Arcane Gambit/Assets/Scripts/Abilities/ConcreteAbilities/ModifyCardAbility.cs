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
    Diamond,
    NewCardFromDeck
}

public class ModifyCardAbility : AbstractAbility
{
    
    private IAbilityInput<Card> _input;
    private IAbilityInput<ModifyKey> _modify;
    

    public ModifyCardAbility(IAbilityInput<Card> cardToModify, IAbilityInput<ModifyKey> modification)
    {
        // Check if the inputs are null
        if (cardToModify == null || modification == null)
        {
            throw new ArgumentNullException(nameof(cardToModify), "The provided arguments must not be null.");
        }

        _input = cardToModify;
        _modify = modification;

        OnInputReceived = () => RequestInput();
    }
    
    /*public ModifyCardAbility(IAbilityInput<Hand> handToModify, IAbilityInput<ModifyKey> modification)
    {
        // Check if the inputs are null
        if (handToModify == null || modification == null)
        {
            throw new ArgumentNullException(nameof(handToModify), "The provided arguments must not be null.");
        }


        List<Card> cards = new List<Card>();
            
        cards = handToModify.GetInput().Cards;
        
        _input = new ConcreteAbilityInput<Card>(null, () => cards[0],
        new ConcreteAbilityInput<Card>(null, () => cards[1],
            new ConcreteAbilityInput<Card>(null, () => cards[2],
                new ConcreteAbilityInput<Card>(null, () => cards[3],
                    new ConcreteAbilityInput<Card>(null, () => cards[4])))));
        
        _modify = modification;

        OnInputReceived = () => RequestInput();
    }*/

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
    
    public override void Dispose()
    {
        Debug.Log("DISPOSE");
        _input.UnsubscribeFromSequence(OnInputReceived);
        _modify.UnsubscribeFromSequence(OnInputReceived);
        _input.Cleanup();
        _modify.Cleanup();
        Finished = false;
    }

    public override void Setup()
    {
        _input.SubscribeToSequence(OnInputReceived);
        _modify.SubscribeToSequence(OnInputReceived);
    }

    public override IEnumerator Process()
    {
        List<CardCollection> cardOwner = new List<CardCollection>();
        foreach (Card card in _input)
        {
            cardOwner.Add(card.Owner);
            Card returnCard = card;
            //Debug.Log(cardOwner);
            cardOwner[cardOwner.Count-1].Remove(card);
            foreach (ModifyKey modify in _modify)
            {
                returnCard = ModifyCard(returnCard, modify); 
            }
            cardOwner[cardOwner.Count-1].AddCard(returnCard);
        }

        foreach (CardCollection cardCollection in cardOwner)
        {
            cardCollection.ResetLeftmostRemoveIndex();
            cardCollection.UpdateVisual();
            
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
            case ModifyKey.NewCardFromDeck:
                return Poker.Instance.Deck.DrawCard();
            default:
                throw new ArgumentOutOfRangeException(nameof(modifyKey), modifyKey, null);
        } 
    }

    public override void RequestInput()
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

    public override bool IsReady()
    {
        return _input.IsInputSequenceReady() && _modify.IsInputSequenceReady();
    }
}
