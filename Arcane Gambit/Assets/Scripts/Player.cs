using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public delegate void PlayerResponseAction(Player player, PlayerRequestType requestType, int value); // delegate definition
    public event PlayerResponseAction OnPlayerResponse;  // Event declaration
    
    public string Name { get; protected set; }
    public Hand Hand { get; protected set; }
    public bool Alive { get; protected set; } = true;

    public bool OutOfBetting { get; protected set; } = false;
    //represents this player's spot in the poker manager, this is here for convenience
    public int IndexInManager { get; }

    public int CurrentBetAmount { get; protected set; }
    public int TotalChips { get; protected set; }
    public CardCollection DiscardedCards { get; protected set; } = new CardCollection();

    public int BidToMatch { get; protected set; } = 0;

    public Player(int handSize, int startingChipAmount, int index, string name = "")
    {
        Hand = new Hand(handSize);
        TotalChips = startingChipAmount;
        IndexInManager = index;
        Name = name;
    }

    public virtual void NewRound()
    {
        Hand.Reset();
        
        OutOfBetting = false;
    }

    public virtual void EndRound()
    {
        BidToMatch = 0;
        CurrentBetAmount = 0;
    }

    public virtual void BidRequest(int bidToMatch)
    {
        BidToMatch = bidToMatch;
    }

    public virtual void MulliganRequest()
    {
        
    }

    public virtual void Match()
    {
        int amount = Math.Max(BidToMatch - CurrentBetAmount, 0);
        TakeChipsForBid(amount);
        RespondToBid(amount);
    }
    public virtual void Raise(int bid = 1)
    {
        if (bid <= 0)
        {
            throw new ArgumentException("Bid amount must be greater than zero.");
        }
        int amount = Math.Max(BidToMatch - CurrentBetAmount, 0) + bid;
        TakeChipsForBid(amount);
        RespondToBid(amount);
    }

    public virtual void Fold()
    {
        OutOfBetting = true;
        RespondToBid(-1);
    }

    protected void TakeChipsForBid(int amount)
    {
        TotalChips -= amount;
        CurrentBetAmount += amount;
    }

    public void GiveChips(int amount)
    {
        TotalChips += amount;
    }

    public void TakeChips(int amount)
    {
        TotalChips -= amount;
    }

    /// <summary>
    /// Invokes the OnPlayerResponse event with the PlayerRequestType.Bid and the specified bidValue.
    /// </summary>
    /// <param name="bidValue">The value indicating the amount to bid.</param>
    public virtual void RespondToBid(int bidValue)
    {
        OnPlayerResponse?.Invoke(this, PlayerRequestType.Bid, bidValue);  //Invoke the event
    }

    public virtual void Mulligan(List<int> cardsToRemove = null)
    {
        if (cardsToRemove == null)
        {
            RespondToMulligan(0);
        }
        else
        {
            DiscardedCards.AddCard(Hand.DiscardCard(cardsToRemove));
            RespondToMulligan(cardsToRemove.Count);
        }
        
    }

    /// <summary>
    /// Responds to a mulligan request from the game manager by invoking the OnPlayerResponse event with the PlayerRequestType.Mulligan and the specified mulliganValue.
    /// </summary>
    /// <param name="mulliganValue">The value indicating the amount of cards the player needs to replace discarded cards.</param>
    public virtual void RespondToMulligan(int mulliganValue)
    {
        OnPlayerResponse?.Invoke(this, PlayerRequestType.Mulligan, mulliganValue);  //Invoke the event
    }
}