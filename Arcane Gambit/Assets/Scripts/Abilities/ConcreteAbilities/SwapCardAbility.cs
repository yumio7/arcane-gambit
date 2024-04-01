using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCardAbility : IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; } = "Swap Card";

    private IAbilityInput<Card> _firstCard;

    public SwapCardAbility(IAbilityInput<Card> firstCard, IAbilityInput<Card> secondCard)
    {
        _firstCard = firstCard;
        _firstCard.LinkSequence(secondCard);
    }

    public void Setup()
    {
        throw new System.NotImplementedException();
    }

    public void Activate()
    {
        AbilityManager.Instance.StartCoroutine(this.Process());
    }

    public IEnumerator Process()
    {
        Card card1 = _firstCard.GetInput();
        Card card2 = _firstCard.NextInput().GetInput();
        CardCollection hand1 = card1.Owner;
        CardCollection hand2 = card2.Owner;
        hand1.Remove(card1);
        hand2.Remove(card2);
        hand1.AddCard(card2);
        hand2.AddCard(card1);
        yield return null;
    }

    public void Finish()
    {
        Debug.Log("done!");
    }

    public void Cleanup()
    {
        throw new System.NotImplementedException();
    }

    public void RequestInput()
    {
        throw new System.NotImplementedException();
    }

    public bool IsReady()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFinished()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
