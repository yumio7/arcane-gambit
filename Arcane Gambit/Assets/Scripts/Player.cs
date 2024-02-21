using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public delegate void PlayerResponseAction(Player player, PlayerRequestType requestType, int value); // delegate definition
    public event PlayerResponseAction OnPlayerResponse;  // Event declaration
    
    public string Name { get; private set; }
    public Hand Hand { get; private set; }
    public bool Alive { get; private set; } = true;

    public bool OutOfBetting { get; private set; } = false;
    //represents this player's spot in the poker manager, this is here for convenience
    public int IndexInManager { get; }

    public int CurrentBetAmount { get; private set; }
    public int TotalChips { get; private set; }
    public List<Card> DiscardedCards { get; private set; } = new List<Card>();

    public int BidToMatch { get; private set; }= 0;

    public Player(int handSize, int startingChipAmount, int index, string name = "")
    {
        Hand = new Hand(handSize);
        TotalChips = startingChipAmount;
        IndexInManager = index;
        Name = name;
    }

    public void NewRound()
    {
        Hand.Reset();
        OutOfBetting = false;
    }

    public void BidRequest(int bidToMatch)
    {
        BidToMatch = bidToMatch;
    }
    
    public void MulliganRequest()
    {
        
    }

    public void Match()
    {
        int amount = BidToMatch - CurrentBetAmount;
        TakeChipsForBid(amount);
        RespondToBid(amount);
    }

    public void Raise(int bid)
    {
        int amount = BidToMatch + bid;
        TakeChipsForBid(amount);
        RespondToBid(amount);
    }

    public void Fold()
    {
        OutOfBetting = true;
        RespondToBid(-1);
    }

    private void TakeChipsForBid(int amount)
    {
        TotalChips -= amount;
        CurrentBetAmount += amount;
    }

    /// <summary>
    /// Invokes the OnPlayerResponse event with the PlayerRequestType.Bid and the specified bidValue.
    /// </summary>
    /// <param name="bidValue">The value indicating the amount to bid.</param>
    public void RespondToBid(int bidValue)
    {
        OnPlayerResponse?.Invoke(this, PlayerRequestType.Bid, bidValue);  //Invoke the event
    }

    /// <summary>
    /// Responds to a mulligan request from the game manager by invoking the OnPlayerResponse event with the PlayerRequestType.Mulligan and the specified mulliganValue.
    /// </summary>
    /// <param name="mulliganValue">The value indicating the amount of cards the player needs to replace discarded cards.</param>
    public void RespondToMulligan(int mulliganValue)
    {
        OnPlayerResponse?.Invoke(this, PlayerRequestType.Mulligan, mulliganValue);  //Invoke the event
    }
}
