using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class GameState
{
    public virtual void OnEnter(Poker poker) { }
    public abstract void Execute(Poker poker);
    public virtual void OnExit(Poker poker) { }
}

public class WaitingState : GameState
{
    public override void Execute(Poker poker)
    {
        // Logic for Waiting
    }
}

public class RoundStartState : GameState
{
    public override void OnEnter(Poker poker)
    {
        poker.PauseProcesses();
        // Perform any necessary setup or initialization for the RoundStartState
        poker.Deck.Shuffle();
        foreach (Player player in poker.Players)
        {
            poker.DealCardToPlayer(player, 5);
        }
        //when animations and timed processes are added, unpausing and pausing will be handled in poker class
        poker.UnpauseProcesses();
    }
    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState();
    }
}

public class BettingRoundState : GameState
{
    public override void OnEnter(Poker poker)
    {
        poker.StartCoroutine(poker.PlayerBettingSequence());
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState();
    }
}

public class MulliganRoundState : GameState
{
    public override void OnEnter(Poker poker)
    {
        //wait for 
    }
    public override void Execute(Poker poker)
    {
        Debug.Log("test");
    }
}

public class CommunityCardState : GameState
{
    public override void Execute(Poker poker)
    {
        
    }
}

public class RoundEndState : GameState
{
    public override void Execute(Poker poker)
    {
        
    }

    public override void OnExit(Poker poker)
    {
        foreach (Player player in poker.Players)
        {
            player.NewRound();
        }
        poker.NewRound();
    }
}

public class GameOverState : GameState
{
    public override void Execute(Poker poker)
    {
        
    }
}

public class CyclicList<T> : List<T>, IEnumerable<T>
{
    private int currentIndex = 0;

    public T Current
    {
        get { return this[currentIndex]; }
    }
    
    public T Next()
    {
        currentIndex = (currentIndex + 1) % Count; // Safely increment with wrap-around
        return this[currentIndex];
    }

    new public IEnumerator<T> GetEnumerator() // New keyword to hide the original GetEnumerator method
    {
        int index = currentIndex;

        do
        {
            yield return this[index];
            index = (index + 1) % Count; // Safely increment with wrap-around
        }
        while (index != currentIndex); // Will loop until it completes a full cycle
    }

    // Explicit interface implementation
    IEnumerator IEnumerable.GetEnumerator()
    { 
        return GetEnumerator();
    }
    
    public void SetCurrentIndex(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException($"Current index at {index} in {this} cannot be set out of bounds");
        }
        currentIndex = index;
    }
}

public enum PlayerRequestType
{
    Bid,
    Mulligan
    // add more request types as needed
}

public class Poker : MonoBehaviour
{
    public GameState PokerState { get; private set; } = new RoundStartState();
    public CyclicList<Player> Players { get; private set; } = new CyclicList<Player>()
    {
        new Player(5, 100, 0, "Player 1"),
        new Player(5, 100, 1,"Player 2"),
        new Player(5, 100, 2,"Player 3"),
        new Player(5, 100, 3,"Player 4")
    };
    public Player CurrentPlayer { get; private set; }
    public Player BlindPlayer { get; private set; }
    public Deck Deck { get; private set; } = new Deck();
    public CardCollection DiscardPile { get; private set; }
    public int BidPot { get; private set; } = 0;
    public int RoundCount { get; private set; } = 1;
    public bool Busy { get; private set; } = false;
    
    private bool _isWaitingForInput = false;
    private readonly CyclicList<GameState> _gameLoopDefinition = new CyclicList<GameState>()
    {
        new RoundStartState(),
        new BettingRoundState(),
        new MulliganRoundState(),
        new BettingRoundState(),
        new CommunityCardState(),
        new BettingRoundState(),
        new RoundEndState()
    };

    private void Awake()
    {
        
    }

    private void Start()
    {
        SwitchState(_gameLoopDefinition[0]);
    }

    void Update()
    {
        
        PokerState.Execute(this);
        
    }

    public void PauseProcesses()
    {
        Busy = true;
    }

    public void UnpauseProcesses()
    {
        Busy = false;
    }

    /// <summary>
    /// Proceeds to the next state of the poker game.
    /// </summary>
    public void ProceedGameState()
    {
        _gameLoopDefinition.Next();
        SwitchState(_gameLoopDefinition.Current);
    }

    public void NewRound()
    {
        Deck.Reset();
        DiscardPile.Reset();
        RoundCount++;
    }
    
    private void SwitchState(GameState gameState)
    {
        PokerState.OnExit(this);
        PokerState = gameState;
        PokerState.OnEnter(this);
    }

    public void DealCardToPlayer(Player player, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            player.Hand.DrawCard(Deck);
        }
    }

    public IEnumerator PlayerBettingSequence()
    {
        PauseProcesses();
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);

        do
        {
            Player player = Players.Current;
            //skip player if they are dead or out of this betting round
            if (!player.Alive || player.OutOfBetting)
            {
                Players.Next();
                continue;
            }
            SetCurrentPlayer(player);
            SendInputRequest(CurrentPlayer, PlayerRequestType.Bid);
            while (_isWaitingForInput)
            {
                yield return null;
            }

            Players.Next();
        } while (!AreAllPlayersMatchingHighestBet());
        
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);
        UnpauseProcesses();
    }

    private void HandlePlayerResponse(Player player, PlayerRequestType requestType, int value) {
        switch(requestType)
        {
            case PlayerRequestType.Bid:
                HandleBidResponse(value);
                break;
            case PlayerRequestType.Mulligan:
                HandleMulliganResponse(player, value);
                break;
            //... additional cases as needed ...
        }
        // continue the coroutine
        _isWaitingForInput = false;
    }
    
    public void HandleBidResponse(int bidValue)
    {
        if (bidValue >= 0)
        {
            BidPot += bidValue;
        }
    }
    public void HandleMulliganResponse(Player player, int mulliganValue)
    {
        DealCardToPlayer(player, mulliganValue);
        
        foreach (Card card in player.DiscardedCards)
        {
            DiscardPile.AddCard(card);
        }
    }

    private void SetCurrentPlayer(Player player) {
        // unsubscribe old player
        if (CurrentPlayer != null) {
            CurrentPlayer.OnPlayerResponse -= HandlePlayerResponse;
        }

        // assign and subscribe new player
        CurrentPlayer = player;
        CurrentPlayer.OnPlayerResponse += HandlePlayerResponse;
    }

    public void SendInputRequest(Player player, PlayerRequestType requestType)
    {
        _isWaitingForInput = true;
        switch (requestType)
        {
            case PlayerRequestType.Bid:
                player.BidRequest();
                break;
            case PlayerRequestType.Mulligan:
                player.MulliganRequest();
                break;
        }
    }

    public bool AreAllPlayersMatchingHighestBet()
    {
        // Determine the highest bet among alive players
        int highestBet = Players.Where(player => player.Alive && !player.OutOfBetting).Max(player => player.CurrentBetAmount);

        // Check if all alive players' current bet equals to the highest bet
        return Players.Where(player => player.Alive && !player.OutOfBetting).All(player => player.CurrentBetAmount == highestBet);
    }
    
}