using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEvalTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Card> hand1 = new List<Card>();
        hand1.Add(new Card(SuitType.Diamond, 14, ""));
        hand1.Add(new Card(SuitType.Diamond, 2, ""));
        hand1.Add(new Card(SuitType.Club, 3, ""));
        hand1.Add(new Card(SuitType.Diamond, 4, ""));
        hand1.Add(new Card(SuitType.Diamond, 5, ""));
        hand1.Add(new Card(SuitType.Diamond, 2, ""));

        List<Card> hand2 = new List<Card>();
        hand2.Add(new Card(SuitType.Diamond, 6, ""));
        hand2.Add(new Card(SuitType.Diamond, 2, ""));
        hand2.Add(new Card(SuitType.Diamond, 3, ""));
        hand2.Add(new Card(SuitType.Diamond, 4, ""));
        hand2.Add(new Card(SuitType.Club, 5, ""));

        int hand1_score = HandEvaluator.EvaluateHand(hand1);
        int hand2_score = HandEvaluator.EvaluateHand(hand2);

        Debug.Log("Hand 1 Score: " + hand1_score);
        Debug.Log("Hand 2 Score: " + hand2_score);

        if(hand1_score > hand2_score)
        {
            Debug.Log("Hand 1 wins!");
        } else if (hand2_score > hand1_score)
        {
            Debug.Log("Hand 2 wins!");
        } else
        {
            Debug.Log("Hands tie!");
        }
    }
}
