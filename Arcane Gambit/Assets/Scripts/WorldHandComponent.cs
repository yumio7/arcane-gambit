using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component used to generate physical cards in the game world
// based of a hand of cards. Supports adding and removing cards 
// to the hand as well as clearing the hand. 
public class WorldHandComponent : MonoBehaviour
{
    [Header("Visual settings")]
    public Transform card_anchor;
    public float card_width = 1f;
    public float card_spacing = 2f;
    public float smooth_time = 5f;

    // Internal
    private GameObject card_base_prefab;
    private List<WorldHandCardObject> cards_in_hand;

    // Represents a card in the hand
    private class WorldHandCardObject
    {
        public Card card;
        public GameObject prefab;
        public Vector3 target_position;

        public WorldHandCardObject(Card card, GameObject prefab, Vector3 target_position)
        {
            this.card = card;
            this.prefab = prefab;
            this.target_position = target_position;
        }
    }

    void Start()
    {
        card_base_prefab = Resources.Load<GameObject>("BaseCardPrefab");
        cards_in_hand = new List<WorldHandCardObject>();
    }

    // Adds a new card to the hand component.
    public void AddCard(Card card)
    {
        if (card_base_prefab != null && card_anchor != null)
        {
            // Create new prefab for card
            GameObject newCardPrefab = Instantiate(card_base_prefab);
            newCardPrefab.transform.position = card_anchor.position;
            newCardPrefab.transform.rotation = card_anchor.rotation;
            newCardPrefab.transform.SetParent(card_anchor);
            CardVisualizer cardVisualizer = newCardPrefab.GetComponent<CardVisualizer>();
            cardVisualizer.SetCard(card);
            // Add it to our hand using its card as a key
            WorldHandCardObject card_object = new WorldHandCardObject(card, newCardPrefab, card_anchor.transform.position);
            cards_in_hand.Add(card_object);
            // Recalculate card world positions
            CalculateCardPositions();
        }
    }

    // Removes a given card from the hand component.
    public void RemoveCard(Card card)
    {
        for(int i = 0; i < cards_in_hand.Count; i++)
        {
            if (Equals(cards_in_hand[i].card, card))
            {
                Destroy(cards_in_hand[i].prefab);
                cards_in_hand.Remove(cards_in_hand[i]);
                CalculateCardPositions();
                return;
            }
        }
    }

    // Removes all cards from the hand component.
    public void ClearHand()
    {
        for (int i = 0; i < cards_in_hand.Count; i++)
        {
            Destroy(cards_in_hand[i].prefab);
        }
        cards_in_hand.Clear();
    }

    // Determines positions of cards in hand 
    private void CalculateCardPositions()
    {
        if (card_anchor != null)
        {
            Vector3 start_offset_pos = card_anchor.position - ((((card_width + card_spacing) * (cards_in_hand.Count - 1)) / 2) * card_anchor.right);
            for (int i = 0; i < cards_in_hand.Count; i++)
            {
                cards_in_hand[i].target_position = start_offset_pos + ((i * (card_width + card_spacing)) * (card_anchor.right));
            }
        }
    }

    // Smoothly position cards based on their target positions.
    private void UpdateCardPositions()
    {
        if (cards_in_hand.Count > 0)
        {
            for (int i = 0; i < cards_in_hand.Count; i++)
            {
                if (cards_in_hand[i].prefab.transform.position != cards_in_hand[i].target_position)
                {
                    cards_in_hand[i].prefab.transform.position = Vector3.Lerp(cards_in_hand[i].prefab.transform.position,
                        cards_in_hand[i].target_position, Time.deltaTime * smooth_time);
                }
            }
        }
    }

    void Update()
    {
        UpdateCardPositions();

        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            AddCard(new Card(SuitType.Spade, 5, ""));
            AddCard(new Card(SuitType.Diamond, 10, ""));
            AddCard(new Card(SuitType.Heart, 14, ""));
        }*/
    }
}
