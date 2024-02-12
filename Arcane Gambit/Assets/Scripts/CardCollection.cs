using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
/// Represents a generic collection of cards
///</summary>
public class CardCollection
{
    public List<Card> Cards { get; private set; }

    public CardCollection()
    {
        Cards = new List<Card>();
    }

    public virtual Card DrawCard()
    {
        var card = Cards.First();
        Cards.RemoveAt(0);
        return card;
    }

    public virtual void Reset()
    {
        Cards.Clear();
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
    }
}