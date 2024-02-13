using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEvalTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Card> hand = new List<Card>();
        hand.Add(new Card(SuitType.Diamond, 13, ""));
        hand.Add(new Card(SuitType.Club, 13, ""));
        hand.Add(new Card(SuitType.Diamond, 3, ""));
        hand.Add(new Card(SuitType.Diamond, 4, ""));
        hand.Add(new Card(SuitType.Diamond, 5, ""));
        hand.Add(new Card(SuitType.Heart, 13, ""));

        Debug.Log("Hand Score: " + HandEvaluator.EvaluateHand(hand));
    }
}
