using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

///<summary>
/// Represents a player's hand of cards
///</summary>
public class Hand : CardCollection
{
    public delegate void HandUpdated(Hand reference);
    public event HandUpdated OnHandUpdated;

    public int MaximumHandSize { get; private set; }

    public Hand(int maximumHandSize)
    {
        MaximumHandSize = maximumHandSize;
    }

    public Hand(Hand otherHand)
    {
        MaximumHandSize = otherHand.MaximumHandSize;
        foreach (var card in otherHand.Cards)
        {
            Cards.Add(new Card(card));
        }
    }

    public void DrawCard(Deck deck)
    {
        if (Cards.Count < MaximumHandSize)
        {
            var card = deck.DrawCard();
            card.Owner = this;
            Cards.Add(card);
            OnHandUpdated?.Invoke(this);
        }
        else
        {
            Debug.Log("The hand is full, can't draw more cards");
        }
    }
    
    public Card DiscardCard(int index)
    {
        if (index >= 0 && index < Cards.Count)
        {
            var card = Cards[index];
            Cards.RemoveAt(index);
            card.Owner = null;
            OnHandUpdated?.Invoke(this);
            return card;
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }
    
    public Card DiscardCard(Card card)
    {
        if (!Cards.Remove(card))
        {
            Debug.Log($"{card} is not in hand");
        }
        card.Owner = null;
        OnHandUpdated?.Invoke(this);
        return card;
    }
    
    public List<Card> DiscardCard(List<int> indices)
    {
        indices.Sort();
        indices.Reverse();
        List<Card> discardedCards = new List<Card>();

        for (int i = 0; i < indices.Count; i++)
        {
            int index = indices[i];
            if (index >= 0 && index < Cards.Count)
            {
                discardedCards.Add(DiscardCard(index));
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
        OnHandUpdated?.Invoke(this);
        return discardedCards;
    }
    
    public List<Card> DiscardCard(List<Card> cardsToRemove)
    {
        List<Card> discardedCards = new List<Card>();

        foreach (Card card in cardsToRemove)
        {
            discardedCards.Add(DiscardCard(card));
        }
        
        OnHandUpdated?.Invoke(this);
        return discardedCards;
    }

    public void PlayCard(Card card)
    {
        if (Cards.Contains(card))
        {
            Cards.Remove(card);
            card.Owner = null;
            OnHandUpdated?.Invoke(this);
        }
        else
        {
            Debug.Log("This card is not in the hand");
        }
    }

    public void UpdateVisual()
    {
        OnHandUpdated?.Invoke(this);
    }
}