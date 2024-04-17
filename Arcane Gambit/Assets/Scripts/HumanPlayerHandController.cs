using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayerHandController : MonoBehaviour
{
    [Header("Human Player World Hand Component")]
    public WorldHandComponent humanPlayerHandComponent;
    [Header("PlayerUI Component")]
    public PlayerUIController playerUIController;

    private void Start()
    {
        if (Poker.Instance != null)
        {
            Poker.Instance.OnPokerStateChanged += RespondToPokerStateChange;
        }
    }

    // Responds to a change in the poker state to bind player hand functionality
    void RespondToPokerStateChange(GameState state)
    {
        if (humanPlayerHandComponent != null)
        {
            List<WorldHandComponent.WorldHandCardObject> hand = humanPlayerHandComponent.GetCardsInHand();
            // Bind player cards to mulligan code
            if (state is MulliganRoundState)
            {
                for (int i = 0; i < hand.Count; i++)
                {
                    EventTriggerClickable clickable = hand[i].prefab.GetComponent<EventTriggerClickable>();
                    clickable.SetEventTriggerParameters(SelectCardForMulligan, i);
                    clickable.SetIsClickable(true);
                }
            } else
            {
                // Cards should not be clickable aside from during mulligan phase
                for (int i = 0; i < hand.Count; i++)
                {
                    EventTriggerClickable clickable = hand[i].prefab.GetComponent<EventTriggerClickable>();
                    clickable.SetIsClickable(false);
                }
            }
        }
    }

    //Toggles a player card for mulligan through the UI controller
    private void SelectCardForMulligan(object value)
    {
        if (playerUIController != null)
        {
            playerUIController.TogglePlayerCardForMulligan((int)value);
        }
    }
}
