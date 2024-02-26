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
        foreach (Player player in poker.Players)
        {
            player.NewRound();
        }
        poker.NewRound();
        poker.PauseProcesses();
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
        poker.StartBettingSequence();
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState();
    }
}

public class BlindBettingRoundState : GameState
{
    public override void OnEnter(Poker poker)
    {
        poker.StartBettingSequence(true);
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
        poker.StartMulliganSequence();
    }
    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState();
    }
}

public class CommunityCardState : GameState
{
    public override void OnEnter(Poker poker)
    {
        poker.UpdateCommunityCard();
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState();
    }
}

public class RoundEndState : GameState
{
    public override void OnEnter(Poker poker)
    {
        poker.EndRound();
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState();
    }

    public override void OnExit(Poker poker)
    {
        
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
        if(Count == 0)
        {
            throw new InvalidOperationException("Cannot get next item. Collection is empty.");
        }

        currentIndex = (currentIndex + 1) % Count;
        return this[currentIndex];
    }

    new public IEnumerator<T> GetEnumerator()
    {
        if(Count == 0)
        {
            yield break;
        }

        int index = currentIndex;

        do
        {
            yield return this[index];
            index = (index + 1) % Count;
        }
        while (index != currentIndex);
    }

    IEnumerator IEnumerable.GetEnumerator()
    { 
        return GetEnumerator();
    }

    public new void Add(T item)
    {
        base.Add(item);
    }

    public new void Remove(T item)
    {
        if(!Contains(item)) 
        {
            throw new ArgumentException("Item not found in list.");
        }

        int removedItemIndex = IndexOf(item);
        base.Remove(item);
        
        if (removedItemIndex <= currentIndex && Count > 0)
        {
            currentIndex--;
        }
    }

    public new void RemoveAt(int index)
    {
        if(index < 0 || index > Count - 1)
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }

        base.RemoveAt(index);

        if (index <= currentIndex && Count > 0)
        {
            currentIndex--;
        }
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
    public CardCollection DiscardPile { get; private set; } = new CardCollection();
    public Card CommunityCard { get; private set; }
    public int BidPot { get; private set; } = 0; 
    public int CurrentMinBid { get; private set; } = 1;
    public int RoundCount { get; private set; } = 0;
    public bool Busy { get; private set; } = false;
    public IBlindSetting Blind;

    private Coroutine _currentProcess;
    private bool _isWaitingForInput = false;
    private readonly CyclicList<GameState> _gameLoopDefinition = new CyclicList<GameState>()
    {
        new RoundStartState(),
        new BlindBettingRoundState(),
        new MulliganRoundState(),
        new BettingRoundState(),
        new CommunityCardState(),
        new BettingRoundState(),
        new RoundEndState()
    };

    #region TEMPORARY

    [SerializeField] private WorldHandComponent _worldHandComponent;

    #endregion

    private void OnEnable()
    {
        foreach (Player player in Players)
        {
            player.OnPlayerResponse += HandlePlayerResponse;
        }
        
    }

    private void OnDisable()
    {
        foreach (Player player in Players)
        {
            player.OnPlayerResponse -= HandlePlayerResponse;
        }
    }

    private void Start()
    {
        //TODO: set blind player randomly
        BlindPlayer = Players[0];

        Blind = new IncrementalBlind(this);
        SwitchState(_gameLoopDefinition[0]);
    }

    void Update()
    {
        #region TEMPORARY_INPUT

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentPlayer.Match();
        }

        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentPlayer.Mulligan(new List<int>() {0});
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentPlayer.Mulligan(new List<int>() {1});
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentPlayer.Mulligan(new List<int>() {2});
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentPlayer.Mulligan(new List<int>() {3});
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CurrentPlayer.Mulligan(new List<int>() {4});
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CurrentPlayer.Mulligan();
        }

        #endregion
        
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

    public void EndRound()
    {
        if (_gameLoopDefinition.OfType<CommunityCardState>().Any())
        {
            
        }
        
        Player winningPlayer = GetWinningPlayer();
        if (winningPlayer != null)
        {
            winningPlayer.GiveChips(BidPot);
            BidPot = 0;
        }
        UpdateBlindStatus();
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

    public void StartBettingSequence(bool forceBlind = false)
    {
        if (_currentProcess != null && Busy)
        {
            StopCoroutine(_currentProcess);
        }
        _currentProcess = StartCoroutine(BettingSequenceCoroutine(forceBlind));
    }
    
    public IEnumerator BettingSequenceCoroutine(bool forceBlind)
    {
        PauseProcesses();

        bool hasCompletedFullLoop = false;
        yield return null;
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);
        Debug.Log("blind: " + BlindPlayer.Name);
        if (forceBlind)
        {
            ProcessBlind();
            Players.Next();
        }
        
        do
        {
            
            //Debug.Log("betting");
            Player player = Players.Current;
            Debug.Log(player.Name);
            //skip player if they are dead or out of this betting round
            if (!player.Alive || player.OutOfBetting)
            {
                Players.Next();
                continue;
            }
            SetCurrentPlayer(player);
            SendInputRequest(CurrentPlayer, PlayerRequestType.Bid);
            //Debug.Log("waiting for " + );
            while (_isWaitingForInput)
            {

                yield return null;
            }

            Players.Next();
            if (Players.Current == BlindPlayer)
            {
                hasCompletedFullLoop = true;
            }
        } while (!AreAllPlayersMatchingHighestBet() || !hasCompletedFullLoop);
        
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);
        Debug.Log("finish");
        UnpauseProcesses();
    }
    
    public void StartMulliganSequence()
    {
        if (_currentProcess != null && Busy)
        {
            StopCoroutine(_currentProcess);
        }
        _currentProcess = StartCoroutine(MulliganSequenceCoroutine());
    }
    
    public IEnumerator MulliganSequenceCoroutine()
    {
        PauseProcesses();
        yield return null;
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);

        foreach (Player player in Players)
        {

            if (!player.Alive || player.OutOfBetting)
            {
                continue;
            }
            SetCurrentPlayer(player);

            #region TEMPORARY_HAND_DISPLAY

            _worldHandComponent?.ClearHand();
            foreach (Card card in CurrentPlayer.Hand.Cards)
            {
                _worldHandComponent?.AddCard(card);
            }

            #endregion
            
            
            SendInputRequest(CurrentPlayer, PlayerRequestType.Mulligan);
            while (_isWaitingForInput)
            {
                //Debug.Log("waiting");
                yield return null;
            }

        }
        
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);
        Debug.Log("finish");
        UnpauseProcesses();
    }

    private void ProcessBlind()
    {
        BlindPlayer.BidRequest(CurrentMinBid);
        BlindPlayer.Match();
    }
    
    public void UpdateBlindStatus()
    {
        this.Blind.UpdateBlind();
    }

    public void SetBlindPlayer(int player)
    {
        this.BlindPlayer = Players[player];
    }

    public void IncreaseBlind()
    {
        this.CurrentMinBid++;
    }

    public void SetCurrentIndexForPlayers(int index)
    {
        this.Players.SetCurrentIndex(index);
    }

    public void MoveToNextPlayerInPlayers()
    {
        this.Players.Next();
    }

    public Player GetNextAlivePlayerInPlayers()
    {
        foreach (Player player in this.Players)
        {
            if (player.Alive)
            {
                return player;
            }
        }

        return null;
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
        CurrentMinBid = Mathf.Max(CurrentMinBid, bidValue);
        if (bidValue >= 0)
        {
            BidPot += bidValue;
        }
    }
    public void HandleMulliganResponse(Player player, int mulliganValue)
    {
        DealCardToPlayer(player, mulliganValue);
        
        foreach (Card card in player.DiscardedCards.Cards)
        {
            DiscardPile.AddCard(card);
        }
    }

    private void SetCurrentPlayer(Player player) {
        CurrentPlayer = player;
    }

    public void SendInputRequest(Player player, PlayerRequestType requestType)
    {
        _isWaitingForInput = true;
        switch (requestType)
        {
            case PlayerRequestType.Bid:
                player.BidRequest(CurrentMinBid);
                break;
            case PlayerRequestType.Mulligan:
                player.MulliganRequest();
                break;
        }
    }

    public bool AreAllPlayersMatchingHighestBet()
    {
        // Determine the highest bet among alive players
        //int highestBet = Players.Where(player => player.Alive && !player.OutOfBetting).Max(player => player.CurrentBetAmount);

        // Check if all alive players' current bet equals to the highest bet
        return Players.Where(player => player.Alive && !player.OutOfBetting).All(player => player.CurrentBetAmount == CurrentMinBid);
    }

    public Player GetWinningPlayer()
    {
        Player highestScoringPlayer = null;
        int highestScore = 0;

        foreach (Player player in Players)
        {
            if (player.Alive && !player.OutOfBetting)
            {
                if (_gameLoopDefinition.OfType<CommunityCardState>().Any())
                {
                    player.Hand.AddCard(CommunityCard);
                }
                
                int score = HandEvaluator.EvaluateHand(player.Hand.Cards);
                
                if (score > highestScore)
                {
                    highestScore = score;
                    highestScoringPlayer = player;
                }
            }
        }

        return highestScoringPlayer;
    }

    public void UpdateCommunityCard()
    {
        CommunityCard = Deck.DrawCard();
    }
    
}

public interface IBlindSetting
{
    public Player UpdateBlind();
    //public int ReturnMinimumBet();
}

public class IncrementalBlind : IBlindSetting
{
    //public int ForceBetAmount { get; private set; }
    public Player BlindOrigin { get; private set; }
    private Poker _poker;
    
    public IncrementalBlind(Poker poker)
    {
        _poker = poker;
        BlindOrigin = _poker.BlindPlayer;
        //ForceBetAmount = initialBlind;
    }

    public Player UpdateBlind()
    {

        UpdateOrigin();
        
        _poker.SetCurrentIndexForPlayers(_poker.BlindPlayer.IndexInManager);
        
        _poker.MoveToNextPlayerInPlayers();

        Player nextPlayer = _poker.GetNextAlivePlayerInPlayers();

        _poker.SetBlindPlayer(nextPlayer.IndexInManager);
        
        if (nextPlayer.Name.Equals(BlindOrigin.Name))
        {
            _poker.IncreaseBlind();
        }

        return nextPlayer;
    }

    private void UpdateOrigin()
    {
        Player nextPlayer = BlindOrigin;
        if (!BlindOrigin.Alive)
        {
            _poker.Players.SetCurrentIndex(BlindOrigin.IndexInManager);
            _poker.MoveToNextPlayerInPlayers();
            nextPlayer = _poker.GetNextAlivePlayerInPlayers();
        }
        if (nextPlayer == null || nextPlayer.Alive == false)
        {
            Debug.LogError("There are no alive players");
        }

        BlindOrigin = nextPlayer;
    }

    /*public int ReturnMinimumBet()
    {
        return ForceBetAmount;
    }

    private void IncreaseBetAmount()
    {
        ForceBetAmount++;
    }*/
}