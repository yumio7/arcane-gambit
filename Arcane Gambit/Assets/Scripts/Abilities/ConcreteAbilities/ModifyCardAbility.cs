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
    
    public ModifyCardAbility(IAbilityInput<Card> cardToModify, IAbilityInput<ModifyKey> modification)
    {
        _input = cardToModify;
        _modify = modification;
        //_input.LinkSequence(input2);
    }
    
    public void Activate(AbilityManager abilityManager)
    {
        abilityManager.StartCoroutine(this.Process(abilityManager));
    }

    public IEnumerator Process(AbilityManager abilityManager)
    {
        foreach (Card card in _input)
        {
            CardCollection cardOwner = card.Owner;
            Card returnCard = card;
            cardOwner.Remove(card);
            foreach (ModifyKey modify in _modify)
            {
                returnCard = ModifyCard(returnCard, modify); 
            }
            cardOwner.AddCard(returnCard);
        }

        yield return null;
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

    public void Finish(AbilityManager abilityManager)
    {
        throw new System.NotImplementedException();
    }

    public void RequestInput()
    {
        throw new System.NotImplementedException();
    }
}
