using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunityCardController : MonoBehaviour
{
    [Header("World Hand Component")]
    public WorldHandComponent worldHandComponent;

    void Update()
    {
        if (Poker.Instance != null && worldHandComponent != null)
        {
            // If the community card is active 
            if (Poker.Instance.CommunityCard != null)
            {
                if (worldHandComponent.CardsInHand() <= 0)
                {
                    worldHandComponent.AddCard(Poker.Instance.CommunityCard);
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
