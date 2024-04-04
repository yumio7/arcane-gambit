using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
/// Represents a deck of cards
///</summary>
public class Deck : CardCollection
{
    public Deck()
    {
        AddStandardDeck();
    }

    // Constructor that takes a deck and creates a deepcopy
    public Deck(Deck otherDeck)
    {
        this.Cards = new List<Card>(otherDeck.Cards.Select(card => new Card(card.Suit, card.Rank)));
    }
    
    public override void Reset()
    {
        base.Reset();
        AddStandardDeck();
    }

    public void AddStandardDeck()
    {
        foreach (var suit in new[] { SuitType.Spade, SuitType.Heart, SuitType.Diamond, SuitType.Club })
        foreach (var rank in new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 })
            AddCard(new Card(suit, rank));
    }

    public void Shuffle()
    {
        for (int i = Cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = Cards[i];
            Cards[i] = Cards[randomIndex];
            Cards[randomIndex] = temp;
        }
    }
} 