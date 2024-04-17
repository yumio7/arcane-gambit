using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("Human Player Hand")]
    public WorldHandComponent humanPlayerHandComponent;
    [Header("UI References")]
    public Text chips_text;
    public Text pot_total_text;
    public Text bet_to_you_text;
    public GameObject bet_ui;
    public Text match_text;
    public Text bet_raise_text;
    public Slider raise_slider;
    [FormerlySerializedAs("raise_ammount_text")] public Text raise_amount_text;
    [FormerlySerializedAs("muligan_ui")] public GameObject mulligan_ui;
    public CardMulliganIcon[] card_select_icons;
    public Image table_turn_indic_icon;
    public Slider hand_strength_slider;
    [Header("Graphics")]
    public Sprite unchecked_sprite;
    public Sprite checked_sprite;
    public Sprite[] turn_sprites;

    // Internal vars
    private Player human_player;
    private int current_raise;
    private List<int> cards_to_remove;

    [System.Serializable]
    public struct CardMulliganIcon
    {
        public Image sprite;
        public GameObject prefab;
    }

    private void Start()
    {
        cards_to_remove = new List<int>();
        FindHumanPlayer();
    }

    private void Update()
    {
        if (Poker.Instance != null && human_player != null)
        {
            if (chips_text != null)
            {
                chips_text.text = "Current Chips: " + human_player.TotalChips.ToString();
            }
            // State specific UI (bid menu vs muligan menu)
            GameState current_state = Poker.Instance.PokerState;
            if (Poker.Instance.CurrentPlayer == human_player)
            {
                if (current_state is BettingRoundState || current_state is BlindBettingRoundState)
                {
                    if (bet_ui != null && mulligan_ui != null)
                    {
                        bet_ui.SetActive(true);
                        mulligan_ui.SetActive(false);
                    }       
                } else if (current_state is MulliganRoundState)
                {
                    if (bet_ui != null && mulligan_ui != null)
                    {
                        bet_ui.SetActive(false);
                        mulligan_ui.SetActive(true);
                    }
                }
            } else
            {
                if (bet_ui != null && mulligan_ui != null)
                {
                    bet_ui.SetActive(false);
                    mulligan_ui.SetActive(false);
                }
            }
            // Raise control
            if (raise_slider != null)
            {
                raise_slider.minValue = Poker.Instance.CurrentMinBid - human_player.CurrentBetAmount + 1;
                raise_slider.maxValue = human_player.TotalChips;
                if (raise_amount_text != null)
                {
                    raise_amount_text.text = raise_slider.value.ToString();
                }
                current_raise = ((int)raise_slider.value) - (Poker.Instance.CurrentMinBid - human_player.CurrentBetAmount);
            }
            // Table info
            if (bet_to_you_text != null && pot_total_text != null && match_text != null && bet_raise_text != null)
            {
                int match_ammount = Mathf.Max(Poker.Instance.CurrentMinBid - human_player.CurrentBetAmount, 0);

                // | Highest Bid: {poker.CurrentMinBid}
                pot_total_text.text = $"Pot Total: {Poker.Instance.BidPot} | (Your round bet: {human_player.CurrentBetAmount})";
                bet_to_you_text.text = $"Phase: {Poker.Instance.PokerState}";

                if (match_ammount > 0)
                {
                    match_text.text = $"MATCH: {match_ammount}";
                    bet_raise_text.text = "RAISE";
                } else
                {
                    match_text.text = "CHECK";
                    bet_raise_text.text = "BET";
                }
                
            }
            // Mulligan control
            if (card_select_icons.Length == 5)
            {
                for(int i = 0; i < 5; i++)
                {
                    if (card_select_icons[i].sprite != null)
                    {
                        if (cards_to_remove.Contains(i))
                        {
                            card_select_icons[i].sprite.sprite = checked_sprite;
                        }
                        else
                        {
                            card_select_icons[i].sprite.sprite = unchecked_sprite;
                        }
                    }
                }

                if (humanPlayerHandComponent != null)
                {
                    List<WorldHandComponent.WorldHandCardObject> hand = humanPlayerHandComponent.GetCardsInHand();
                    if (hand.Count == 5)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (card_select_icons[i].prefab != null)
                            {
                                card_select_icons[i].prefab.transform.position = 
                                    Camera.main.WorldToScreenPoint(hand[i].prefab.transform.position + new Vector3(0, 0.3f, 0));
                            }
                        }
                    }
                }
            }
            // Turn indicator
            if (table_turn_indic_icon != null && turn_sprites.Length == 4)
            {
                CyclicList<Player> players = Poker.Instance.Players;
                Player turn_player = Poker.Instance.CurrentPlayer;
                int index = players.IndexOf(turn_player);
                if (index > -1 && index < players.Count) {
                    table_turn_indic_icon.sprite = turn_sprites[players.IndexOf(turn_player)];
                }
            }
            // Hand Strengh Slider
            if (hand_strength_slider != null)
            {
                if (human_player.Hand.Cards != null)
                {
                    hand_strength_slider.minValue = 0;
                    hand_strength_slider.maxValue = 8000014; //8,000,014

                    if (!human_player.OutOfBetting)
                    {
                        List<Card> hand_to_check = new List<Card>(human_player.Hand.Cards);
                        // Check if community card is available or not
                        if (Poker.Instance.CommunityCard != null)
                        {
                            hand_to_check.Add(Poker.Instance.CommunityCard);
                        }
                        int hand_score = HandEvaluator.EvaluateHand(hand_to_check);
                        hand_strength_slider.value = hand_score;
                    } else
                    {
                        hand_strength_slider.value = 0;
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
        if (Poker.Instance != null)
        {
            CyclicList<Player> players = Poker.Instance.Players;
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
