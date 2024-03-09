using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("Poker Instance")]
    public Poker poker;
    [Header("UI References")]
    public Text chips_text;
    public Text pot_total_text;
    public Text bet_to_you_text;
    public GameObject bet_ui;
    public Slider raise_slider;
    public Text raise_ammount_text;
    public GameObject muligan_ui;
    public Image[] card_select_icons;
    [Header("Graphics")]
    public Sprite unchecked_sprite;
    public Sprite checked_sprite;

    // Internal vars
    private Player human_player;
    private int current_raise;
    private List<int> cards_to_remove;

    private void Start()
    {
        cards_to_remove = new List<int>();
        FindHumanPlayer();

        if (poker == null) { Debug.LogWarning("Player UI missing poker reference!"); }
    }

    private void Update()
    {
        if (poker != null && human_player != null)
        {
            if (chips_text != null)
            {
                chips_text.text = "Current Chips: " + human_player.TotalChips.ToString();
            }
            // State specific UI (bid menu vs muligan menu)
            GameState current_state = poker.PokerState;
            if (poker.CurrentPlayer == human_player)
            {
                if (current_state is BettingRoundState || current_state is BlindBettingRoundState)
                {
                    if (bet_ui != null && muligan_ui != null)
                    {
                        bet_ui.SetActive(true);
                        muligan_ui.SetActive(false);
                    }       
                } else if (current_state is MulliganRoundState)
                {
                    if (bet_ui != null && muligan_ui != null)
                    {
                        bet_ui.SetActive(false);
                        muligan_ui.SetActive(true);
                    }
                }
            } else
            {
                if (bet_ui != null && muligan_ui != null)
                {
                    bet_ui.SetActive(false);
                    muligan_ui.SetActive(false);
                }
            }
            // Raise control
            if (raise_slider != null)
            {
                raise_slider.minValue = poker.CurrentMinBid + 1;
                raise_slider.maxValue = human_player.TotalChips;
                if (raise_ammount_text != null)
                {
                    raise_ammount_text.text = raise_slider.value.ToString();
                }
                current_raise = (int)raise_slider.value;
            }
            // Table info
            if (bet_to_you_text != null && pot_total_text != null)
            {
                pot_total_text.text = "Pot Total: " + poker.BidPot;
                bet_to_you_text.text = "Current Bet: " + poker.CurrentMinBid;
            }
            // Mulligan control
            if (card_select_icons.Length == 5)
            {
                for(int i = 0; i < 5; i++)
                {
                    if (cards_to_remove.Contains(i))
                    {
                        card_select_icons[i].sprite = checked_sprite;
                    } else
                    {
                        card_select_icons[i].sprite = unchecked_sprite;
                    }
                }
            }
        }
    }

    public void FoldPlayer()
    {
        if (human_player != null)
        {
            human_player.Fold();
        }
    }

    public void MatchPlayer()
    {
        if (human_player != null)
        {
            human_player.Match();
        }
    }

    public void RaisePlayer()
    {
        if (human_player != null)
        {
            human_player.Raise(current_raise);
        }
    }

    public void ConfirmPlayerMulligan()
    {
        if (human_player != null)
        {
            if (cards_to_remove.Count > 0)
            {
                human_player.Mulligan(cards_to_remove);
                cards_to_remove.Clear();
            } else
            {
                human_player.Mulligan();
            }     
        }
    }

    public void TogglePlayerCardForMulligan(int id)
    {
        if (cards_to_remove.Contains(id))
        {
            cards_to_remove.Remove(id);
        } else
        {
            cards_to_remove.Add(id);
        }
    }

    private void FindHumanPlayer()
    {
        if (poker != null)
        {
            CyclicList<Player> players = poker.Players;
            for(int i = 0; i < players.Count; i++)
            {
                if (players.Current.Type == PlayerType.Human)
                {
                    human_player = players.Current;
                    return;
                } else
                {
                    players.Next();
                }
            }
        }
    }
}
