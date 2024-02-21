using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a computer controlled opponent
/// </summary>
public class Opponent : Player
{
    public Opponent(int handSize, int startingChipAmount, int index, string name = "")
        : base(handSize, startingChipAmount, index, name) { }

    

}