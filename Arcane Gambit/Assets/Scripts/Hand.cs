using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    List<Card> cards;
    int handLimit;

    public Hand ()
    {
        cards = new List<Card> ();
        handLimit = 0;
    }

    public Hand (List<Card> cards, int handLimit)
    {
        this.cards = cards;
        this.handLimit = handLimit;
    }
}
