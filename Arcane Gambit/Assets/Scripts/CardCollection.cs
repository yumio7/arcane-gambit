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
    private int _leftmostRemovedIndex;
    
    public List<Card> Cards { get; protected set; }

    public CardCollection()
    {
        Cards = new List<Card>();
        ResetLeftmostRemoveIndex();
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
        card.Owner = null;
        SetLeftmostRemoveIndex(0);
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
        if(card == null) {return;}
        card.Owner = this;
        if (_leftmostRemovedIndex > Cards.Count)
        {
            ResetLeftmostRemoveIndex();
        }
        Cards.Insert(_leftmostRemovedIndex, card);
    }

    public void AddCard(List<Card> cards)
    {
        if(cards == null || cards.Count == 0) {return;}
        foreach (Card card in cards)
        {
            card.Owner = this;
            if (_leftmostRemovedIndex > Cards.Count)
            {
                ResetLeftmostRemoveIndex();
            }
            Cards.Insert(_leftmostRemovedIndex, card);
        }
    }

    public bool Remove(Card card)
    {
        card.Owner = null;
        SetLeftmostRemoveIndex(Cards.IndexOf(card));
        return Cards.Remove(card);
    }

    public void RemoveAt(int index)
    {
        Cards[index].Owner = null;
        Cards.RemoveAt(index);
        SetLeftmostRemoveIndex(index);
    }

    public void SetLeftmostRemoveIndex(int newIndex)
    {
        _leftmostRemovedIndex = Mathf.Min(_leftmostRemovedIndex, newIndex);
    }

    public void ResetLeftmostRemoveIndex()
    {
        _leftmostRemovedIndex = Cards.Count;
    }
}