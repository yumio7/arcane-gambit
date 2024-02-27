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
    public List<Card> Cards { get; protected set; }

    public CardCollection()
    {
        Cards = new List<Card>();
    }
    
    // Deep copy constructor
    public CardCollection(CardCollection original)
    {
        Cards = original.Cards.Select(card => new Card(card)).ToList();
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

    public override string ToString()
    {
        string hand = "";
        foreach (var card in Cards)
        {
            hand += card.ToString() + ", ";
        }

        return hand;
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
    }

    public void AddCard(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            Cards.Add(card);
        }
    }
}