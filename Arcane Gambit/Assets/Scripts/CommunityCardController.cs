using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunityCardController : MonoBehaviour
{
    [Header("Poker Instance")]
    public Poker poker;
    [Header("World Hand Component")]
    public WorldHandComponent worldHandComponent;

    void Update()
    {
        if (poker != null && worldHandComponent != null)
        {
            // If the community card is active 
            if (poker.CommunityCard != null)
            {
                if (worldHandComponent.CardsInHand() <= 0)
                {
                    worldHandComponent.AddCard(poker.CommunityCard);
                }
            } else
            {
                if (worldHandComponent.CardsInHand() >= 1)
                {
                    worldHandComponent.ClearHand();
                }
            }
        }
    }
}
