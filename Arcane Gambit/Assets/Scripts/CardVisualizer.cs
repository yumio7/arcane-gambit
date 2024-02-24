using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualizer : MonoBehaviour
{
    [Header("Visuals")]
    public Text card_rank_text;
    public Image card_suit_image;
    public Color red_suit_color;
    public Color black_suit_color;
    public Sprite diamond_suit_icon;
    public Sprite heart_suit_icon;
    public Sprite club_suit_icon;
    public Sprite spade_suit_icon;

    public void SetCard(Card card)
    {
        if (card_rank_text != null && card_suit_image != null)
        {
            if (card.Rank > 10)
            {
                card_rank_text.text = (CardGame.RankToString(card.Rank)[0]).ToString();
            } else
            {
                card_rank_text.text = card.Rank.ToString();
            }

            switch(card.Suit)
            {
                case SuitType.Diamond:
                    card_suit_image.sprite = diamond_suit_icon;
                    card_suit_image.color = red_suit_color;
                    card_rank_text.color = red_suit_color;
                    break;
                case SuitType.Heart:
                    card_suit_image.sprite = heart_suit_icon;
                    card_suit_image.color = red_suit_color;
                    card_rank_text.color = red_suit_color;
                    break;
                case SuitType.Club:
                    card_suit_image.sprite = club_suit_icon;
                    card_suit_image.color = black_suit_color;
                    card_rank_text.color = black_suit_color;
                    break;
                case SuitType.Spade:
                    card_suit_image.sprite = spade_suit_icon;
                    card_suit_image.color = black_suit_color;
                    card_rank_text.color = black_suit_color;
                    break;
            }
        }
    }
}
