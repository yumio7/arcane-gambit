using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a computer controlled opponent
/// </summary>
public class Opponent : Player
{
    private PokerData _pokerData;
    
    /// <summary>
    /// Confidence is the main modifier and is preset for each opponent, it's a number between 0 and 10
    /// </summary>
    public int conf { get; }

    /// <summary>
    /// Optimism is the variable parameter, its randomly generated before decisions.
    /// </summary>
    public int opt { get; private set; }

    public Opponent(int handSize, int startingChipAmount, int index, string name = "", int conf = 5)
        : base(handSize, startingChipAmount, index, PlayerType.AI, name) 
    {
        this.conf = conf;
        opt = 5;
    }

    private void calc_bid()
    {
        int bid = 0;
        // the score from the hand evaluator
        int hand_score = HandEvaluator.EvaluateHand(Hand.Cards);

        // normalized hand score between 0 and 1
        float raw_score = Opponent.normalize_score(hand_score);

        // calculate optimism
        this.opt = Random.Range(1, 6) + Random.Range(1,6);

        // apply modifiers
        raw_score *= conf_mod();
        raw_score *= (this.opt * 0.2);

        if (raw_score > 1.85)
        {
            RespondToBid(TotalChips);
        }
        else if (raw_score >= 1.35)
        {
            Raise()
        }
        else if (raw_score < 0.25)
        {
            Fold();
        }
        else 
        {
            Match();
        }
        //TODO: Community card needs to be taken into account
        
        // game info struct could be used to make harder difficulties
    }

    static public float normalize_score(int hand_score)
    {
        return hand_score / 8000014;
    }

    private float conf_mod()
    {
        return Math.Log(conf + 5, 10);
    }

    public override void BidRequest(int bidToMatch)
    {
        base.BidRequest(bidToMatch);
        calc_bid();
    }

    public override void UpdateData(PokerData data)
    {
        _pokerData = data;
    }
}