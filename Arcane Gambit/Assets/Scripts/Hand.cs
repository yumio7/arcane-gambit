using UnityEngine;
using System.Collections.Generic;
using System.Linq;

///<summary>
/// Represents a player's hand of cards
///</summary>
public class Hand : CardCollection
{
    public int MaximumHandSize { get; private set; }

    public Hand(int maximumHandSize)
    {
        MaximumHandSize = maximumHandSize;
    }

    public void DrawCard(Deck deck)
    {
        if (Cards.Count < MaximumHandSize)
        {
            var card = deck.DrawCard();
            Cards.Add(card);
        }
        else
        {
            Debug.Log("The hand is full, can't draw more cards");
        }
    }

    public void PlayCard(Card card)
    {
        if (Cards.Contains(card))
        {
            Cards.Remove(card);
        }
        else
        {
            Debug.Log("This card is not in the hand");
        }
    }
}