using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerHandsManager : MonoBehaviour
{
    [Header("World Hand Components")]
    public WorldHandComponent[] worldHandComponents;

    private List<Player> players_in_game;
    private List<Hand> poker_hands; //a list of references to the hand of each player in the game

    private void Start()
    {
        poker_hands = new List<Hand>();
        players_in_game = new List<Player>();

        if (Poker.Instance != null)
        {
            CyclicList<Player> players = Poker.Instance.Players;
            for (int i = 0; i < players.Count; i++)
            {
                poker_hands.Add(players[i].Hand);
                players[i].Hand.OnHandUpdated += UpdateHandComponent;
                players_in_game.Add(players[i]);
                players[i].OnPlayerFold += FoldHandComponent;
                players.Next();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Put your custom action here
            foreach (Player player in Poker.Instance.Players)
            {
                player.Hand.UpdateVisual();
            }
        }
    }

    // Updates a speicifc hand's world hand component
    private void FoldHandComponent(Player reference)
    {
        if (players_in_game.Contains(reference))
        {
            int index = players_in_game.IndexOf(reference);
            if (index < worldHandComponents.Length)
            {
                if (worldHandComponents[index] != null)
                {
                    worldHandComponents[index].ClearHand();
                }
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