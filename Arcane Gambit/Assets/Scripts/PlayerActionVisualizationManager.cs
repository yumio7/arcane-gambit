using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionVisualizationManager : MonoBehaviour
{
    [Header("Poker Instance")]
    public Poker poker;
    [Header("Message Visualization Components")]
    public WorldMessageVisualizerComponent[] worldMessageVisualizers;

    private List<Player> players_in_game;

    void Start()
    {
        players_in_game = new List<Player>();
        if (poker == null) { Debug.LogWarning("Player UI missing poker reference!"); }

        if (poker != null)
        {
            CyclicList<Player> players = poker.Players;
            for (int i = 0; i < players.Count; i++)
            {
                players_in_game.Add(players[i]);
                players[i].OnPlayerResponse += DisplayPlayerActionResponseMessage;
                players.Next();
            }
        }
    }

    public void DisplayPlayerActionResponseMessage(Player player, PlayerRequestType requestType, int value)
    {
        if (players_in_game.Contains(player) && poker != null)
        {
            int index = players_in_game.IndexOf(player);
            if (index < worldMessageVisualizers.Length)
            {
                if (worldMessageVisualizers[index] != null)
                {
                    string message = player.Name + " ";

                    switch(requestType)
                    {
                        case PlayerRequestType.Bid:
                            if (value == 0)
                            {
                                message += "checked.";
                            } else if (value == -1)
                            {
                                message += "folded.";
                            } else if (players_in_game[index].CurrentBetAmount == poker.CurrentMinBid)
                            {
                                message += "matched the bet.";
                            }
                            else
                            {
                                message += "raised by " + value.ToString() + ".";
                            }
                            break;
                        case PlayerRequestType.Mulligan:
                            if (value > 0)
                            {
                                message += "took " + value + " cards.";
                            } else
                            {
                                message += "didn't take any cards.";
                            }
                            break;
                    }

                    worldMessageVisualizers[index].DisplayMessage(message);
                }
            }
        }
    }
}
