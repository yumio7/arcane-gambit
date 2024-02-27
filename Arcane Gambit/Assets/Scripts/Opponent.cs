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
        opt = 50;
    }

    private void calc_bid()
    {
        int bid = 0;
        int hand_score = HandEvaluator.EvaluateHand(Hand.Cards);
        //TODO: turn this number ^ into something between 0 and 1 using confidence and optimism
        //TODO: calculate optimism2
        
        RespondToBid(bid);
    }

    public override void BidRequest(int bidToMatch)
    {
        base.BidRequest(bidToMatch);
        calc_bid();
    }
}