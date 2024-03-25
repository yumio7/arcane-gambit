using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCardAbility : IAbility
{
    public IAbility NextAbility { get; set; }
    public string AbilityName { get; set; } = "Swap Card";

    private Card _card1;
    private Card _card2;

    public SwapCardAbility(Card card1, Card card2)
    {
        _card1 = card1;
        _card2 = card2;
    }
    
    public void Activate(AbilityManager abilityManager)
    {
        abilityManager.StartCoroutine(this.Process(abilityManager));
    }

    public IEnumerator Process(AbilityManager abilityManager)
    {
        CardCollection hand1 = _card1.Owner;
        CardCollection hand2 = _card2.Owner;
        yield return null;
        hand1.Remove(_card1);
        hand2.Remove(_card2);
        hand1.AddCard(_card2);
        hand2.AddCard(_card1);
    }

    public void Finish(AbilityManager abilityManager)
    {
        Debug.Log("done!");
    }

    public void RequestInput()
    {
        throw new System.NotImplementedException();
    }
}
