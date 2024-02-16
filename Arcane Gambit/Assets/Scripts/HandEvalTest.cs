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
        hand1.Add(new Card(SuitType.Diamond, 3, ""));
        hand1.Add(new Card(SuitType.Diamond, 4, ""));
        hand1.Add(new Card(SuitType.Club, 5, ""));
        //hand1.Add(new Card(SuitType.Diamond, 6, ""));

        Debug.Log("Hand Score: " + HandEvaluator.EvaluateHand(hand1));
    }
}
