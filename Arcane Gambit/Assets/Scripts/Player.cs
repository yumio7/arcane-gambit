using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { Human, AI }
public class Player
{
    public delegate void PlayerResponseAction(Player player, PlayerRequestType requestType, int value); // delegate definition
    public event PlayerResponseAction OnPlayerResponse;  // Event declaration

    public delegate void PlayerFoldAction(Player reference);
    public event PlayerFoldAction OnPlayerFold;

    public string Name { get; protected set; }
    public PlayerType Type { get; protected set; }
    public Hand Hand { get; protected set; }
    public bool Alive { get; protected set; } = true;

    public bool OutOfBetting { get; protected set; } = false;
    //represents this player's spot in the poker manager, this is here for convenience
    public int IndexInManager { get; }
    public int RoundBetAmount { get; protected set; }
    public int CurrentBetAmount { get; protected set; }
    public int TotalChips { get; protected set; }
    public CardCollection DiscardedCards { get; protected set; } = new CardCollection();

    public int BidToMatch { get; protected set; } = 0;

    public Player(int handSize, int startingChipAmount, int index, PlayerType playerType, string name = "")
    {
        Hand = new Hand(handSize);
        TotalChips = startingChipAmount;
        IndexInManager = index;
        Name = name;
        Type = playerType;
    }

    public Player(int handSize, int startingChipAmount, int index, string name = "")
    {
        Hand = new Hand(handSize);
        TotalChips = startingChipAmount;
        IndexInManager = index;
        Name = name;
        Type = PlayerType.AI;
    }
    
    public Player(Player player)
    {
        // Creating deep copy of the provided player
        this.Name = player.Name;
        this.Hand = new Hand(player.Hand);
        this.Alive = player.Alive;
        this.OutOfBetting = player.OutOfBetting;
        this.IndexInManager = player.IndexInManager;
        this.CurrentBetAmount = player.CurrentBetAmount;
        this.TotalChips = player.TotalChips;
        this.DiscardedCards = new CardCollection(player.DiscardedCards);
        this.BidToMatch = player.BidToMatch;
        this.Type = player.Type;
        this.RoundBetAmount = player.RoundBetAmount;
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
        RoundBetAmount = 0;
    }

    public virtual void ResetCurrentBidAmount()
    {
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
        OnPlayerFold?.Invoke(this);
    }

    public virtual void HideHand()
    {
        OnPlayerFold?.Invoke(this);
    }

    public void GiveChips(int amount)
    {
        TotalChips += amount;
    }
    
    protected void TakeChipsForBid(int amount)
    {
        int finalAmount = TotalChips - amount;
        if (finalAmount < 0)
        {
            Die();
            TotalChips = 0;
            return;
        }
        TotalChips = finalAmount;
        CurrentBetAmount += amount;
        RoundBetAmount += amount;
    }

    public void TakeChips(int amount)
    {
        int remainingChips = TotalChips - amount;
        if (remainingChips < 0)
        {
            Die();
            TotalChips = 0;
            return;
        }
        TotalChips = remainingChips;
    }

    public void Die()
    {
        Alive = false;
        OutOfBetting = false;
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

    public virtual void UpdateData(PokerData data)
    {
        
    }

}