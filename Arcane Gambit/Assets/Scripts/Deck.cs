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
        for (int i = 0; i < Cards.Count; i++) 
        {
            Card temp = Cards[i];
            int randomIndex = Random.Range(i, Cards.Count);
            Cards[i] = Cards[randomIndex];
            Cards[randomIndex] = temp;
        }
    }
}
