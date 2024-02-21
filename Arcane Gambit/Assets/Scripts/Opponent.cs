using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a computer controlled opponent
/// </summary>
public class Opponent : Player
{
    /// <summary>
    /// Confidence is the main modifier and is preset for each opponent
    /// </summary>
    public int conf { get; }

    /// <summary>
    /// Optimism is the variable parameter, it changes based on whats going on in the game
    /// </summary>
    public int opt { get; private set; }

    public Opponent(int handSize, int startingChipAmount, int index, string name = "", int conf = 5)
        : base(handSize, startingChipAmount, index, name) 
    {
        // TODO: determine what scale this will be on
        this.conf = conf;
        this.opt = 50;
    }

}