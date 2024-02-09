using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    SuitType suit;
    int rank; //1 - 13 is ace to king
    string name;

    public Card(SuitType suit, int rank, string name = "")
    {
        this.suit = suit;
        this.rank = rank;
        this.name = name;
        if (this.name == "")
        {
            this.name = FormattedName();
        }
    }

    public string FormattedName()
    {
        return $"{CardGame.RankToString(rank)} of {suit}s";
    }

    public void Print()
    {
        Debug.Log(FormattedName());
    }
}
