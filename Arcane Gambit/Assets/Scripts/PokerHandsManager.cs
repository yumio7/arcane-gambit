using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerHandsManager : MonoBehaviour
{
    [Header("Poker Instance")]
    public Poker poker;
    [Header("World Hand Components")]
    public WorldHandComponent[] worldHandComponents;

    private List<Hand> poker_hands; //a list of references to the hand of each player in the game

    private void Start()
    {
        poker_hands = new List<Hand>();
        if (poker == null) { Debug.LogWarning("Player UI missing poker reference!"); }

        if (poker != null)
        {
            CyclicList<Player> players = poker.Players;
            for (int i = 0; i < players.Count; i++)
            {
                poker_hands.Add(players[i].Hand);
                players[i].Hand.OnHandUpdated += UpdateHandComponent;
                players.Next();
            }
        }
    }

    // Updates a speicifc hand's world hand component
    private void UpdateHandComponent(Hand reference)
    {
        if (poker_hands.Contains(reference))
        {
            int index = poker_hands.IndexOf(reference);
            if (index < worldHandComponents.Length)
            {
                if (worldHandComponents[index] != null)
                {
                    worldHandComponents[index].ClearHand();
                    foreach (Card c in poker_hands[index].Cards)
                    {
                        worldHandComponents[index].AddCard(c);
                    }
                }
            }
        }
    }
}
