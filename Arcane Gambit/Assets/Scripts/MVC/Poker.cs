using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class GameState
{
    protected float delay = 1f;
    
    public GameState(float delay = 1f)
    {
        this.delay = delay;
    }
    
    public virtual void OnEnter(Poker poker) { }
    public abstract void Execute(Poker poker);
    public virtual void OnExit(Poker poker) { }
}

public class WaitingState : GameState
{
    public WaitingState(float delay = 1f) : base(delay) { }
    
    public override void Execute(Poker poker)
    {
        // Logic for Waiting
    }
}

public class RoundStartState : GameState
{
    public RoundStartState(float delay = 1f) : base(delay) { }
    
    public override void OnEnter(Poker poker)
    {
        if (poker.VerifyWinOrLose()) { return; }
        foreach (Player player in poker.Players)
        {
            player.NewRound();
        }
        poker.NewRound();
        poker.PauseProcesses();
        poker.Deck.Shuffle();
        foreach (Player player in poker.Players)
        {
            if (!player.Alive) { continue; }
            poker.DealCardToPlayer(player, 5);
        }
        //when animations and timed processes are added, unpausing and pausing will be handled in poker class
        poker.UnpauseProcesses();
    }
    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        Debug.Log(poker.PokerState);
        poker.ProceedGameState(delay);
    }
}

public class BettingRoundState : GameState
{
    public BettingRoundState(float delay = 1f) : base(delay) { }
    
    public override void OnEnter(Poker poker)
    {
        if (poker.VerifyWinOrLose()) { return; }
        poker.StartBettingSequence();
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState(delay);
    }
}

public class BlindBettingRoundState : GameState
{
    public BlindBettingRoundState(float delay = 1f) : base(delay) { }
    
    public override void OnEnter(Poker poker)
    {
        if (poker.VerifyWinOrLose()) { return; }
        poker.StartBettingSequence(true);
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState(delay);
    }
}

public class MulliganRoundState : GameState
{
    public MulliganRoundState(float delay = 1f) : base(delay) { }
    
    public override void OnEnter(Poker poker)
    {
        if (poker.VerifyWinOrLose()) { return; }
        poker.StartMulliganSequence();
    }
    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState(delay);
    }
}

public class CommunityCardState : GameState
{
    public CommunityCardState(float delay = 1f) : base(delay) { }
    
    public override void OnEnter(Poker poker)
    {
        if (poker.VerifyWinOrLose()) { return; }
        poker.UpdateCommunityCard();
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState(delay);
    }
}

public class RoundEndState : GameState
{
    public RoundEndState(float delay = 1f) : base(delay) { }
    
    public override void OnEnter(Poker poker)
    {
        poker.EndRound();
        foreach (Player player in poker.Players)
        {
            player.EndRound();
            player.HideHand();
        }

        poker.VerifyWinOrLose();
    }

    public override void Execute(Poker poker)
    {
        if (poker.Busy) { return; }
        poker.ProceedGameState(delay);
    }

    public override void OnExit(Poker poker)
    {

    }
}

public class GameWinState : GameState
{
    public GameWinState(float delay = 1f) : base(delay) { }

    private EventButtonGenerator.EventButtonData _gameOverText;
    
    public override void OnEnter(Poker poker)
    {
        _gameOverText =
            EventButtonGenerator.Instance.CreateNewEventButton(0, 0, "You Win!", o => Debug.Log(o), "You Win!");
    }

    public override void Execute(Poker poker)
    {
        
    }
}

public class GameLoseState : GameState
{
    public GameLoseState(float delay = 1f) : base(delay) { }

    private EventButtonGenerator.EventButtonData _gameOverText;
    
    public override void OnEnter(Poker poker)
    {
        _gameOverText =
            EventButtonGenerator.Instance.CreateNewEventButton(0, 0, "Game Over!\nThe Wizards took your soul...", o => Debug.Log(o), "Game Over!\nThe Wizards took your soul...");
    }

    public override void Execute(Poker poker)
    {
        
    }
}

public class CyclicList<T> : List<T>, IEnumerable<T>
{
    private int currentIndex;

    public CyclicList() 
    {
    }

    public CyclicList(CyclicList<T> other)
    {
        this.AddRange(other);
        this.currentIndex = other.currentIndex;
    }

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

public struct PokerData
{
    public GameState PokerState;
    public CyclicList<Player> Players;
    public Player CurrentPlayer;
    public Player BlindPlayer;
    public Deck Deck;
    public CardCollection DiscardPile;
    public Card CommunityCard;
    public int BidPot;
    public int CurrentMinBid;
    public int RoundCount;
    public bool Busy;
    public IBlindSetting Blind;
    public CyclicList<GameState> GameLoopDefinition;
}

public class Poker : MonoBehaviour
{
    // Singleton instance
    public static Poker Instance { get; private set; }

    private void Awake()
    {
        // Ensure that there is only one instance of Poker
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public GameState PokerState { get; private set; } = new RoundStartState();
    public CyclicList<Player> Players { get; private set; } = new CyclicList<Player>()
    {

        new Player(5, 100, 0, PlayerType.Human, "Player 1"),
        new Opponent(5, 100, 1),
        new Opponent(5, 100, 2),
        new Opponent(5, 100, 3)
         
        //new Player(5, 100, 0, PlayerType.Human, "Player 1"),
        //new Player(5, 100, 1, PlayerType.AI, "Player 2"),
        //new Player(5, 100, 2, PlayerType.AI, "Player 3"),
        //new Player(5, 100, 3, PlayerType.AI, "Player 4")
    };
    public Player CurrentPlayer { get; private set; }
    public Player BlindPlayer { get; private set; }
    public Deck Deck { get; private set; } = new Deck();
    public CardCollection DiscardPile { get; private set; } = new CardCollection();
    public Card CommunityCard { get; private set; }
    public int BidPot { get; private set; } = 0; 
    public int CurrentMinBid { get; private set; } = 0;
    public int BlindBid { get; private set; } = 1;
    public int RoundCount { get; private set; } = 0;
    public bool Busy { get; private set; } = false;
    public IBlindSetting Blind;

    private Coroutine _currentProcess;
    private bool _isWaitingForInput = false;
    private Coroutine _pauseCoroutine;
    private Coroutine _unpauseCoroutine;
    private Coroutine _proceedGameStateCoroutine;
    private readonly CyclicList<GameState> _gameLoopDefinition = new CyclicList<GameState>()
    {
        new RoundStartState(1),
        new BlindBettingRoundState(0.1f),
        new MulliganRoundState(0.1f),
        new BettingRoundState(0.1f),
        new CommunityCardState(0.5f),
        new BettingRoundState(0.1f),
        new RoundEndState(1)
    };
    
    //Rest of the poker class as defined 

    public PokerData DeepCopy()
    {
        var gameStatesCopy = new CyclicList<GameState>(_gameLoopDefinition);

        var playersCopy = new CyclicList<Player>();
        foreach (var player in Players)
        {
            playersCopy.Add(new Player(player));
        }

        return new PokerData
        {
            PokerState = PokerState,
            Players = playersCopy,
            CurrentPlayer = new Player(CurrentPlayer),
            BlindPlayer = new Player(BlindPlayer),
            Deck = new Deck(Deck),
            DiscardPile = new CardCollection(DiscardPile),
            CommunityCard = CommunityCard,     
            BidPot = BidPot,
            CurrentMinBid = CurrentMinBid,
            RoundCount = RoundCount,
            Busy = Busy,
            Blind = Blind,
            GameLoopDefinition = gameStatesCopy
        };
    }

    public void UpdateAIData()
    {
        PokerData data = DeepCopy();
        foreach (Player player in Players)
        {
            player.UpdateData(data);
        }
    }

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

        if (Busy)
        {
            return;
        }
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

    public void PauseProcesses(float duration)
    {
        if (_pauseCoroutine != null)
        {
            StopCoroutine(_pauseCoroutine);
        }
        _pauseCoroutine = StartCoroutine(PauseProcessesCoroutine(duration));
    }

    private IEnumerator PauseProcessesCoroutine(float duration)
    {
        Busy = true;
        yield return new WaitForSeconds(duration);
        Busy = false;
    }

    public void UnpauseProcesses(float delay)
    {
        if (_unpauseCoroutine != null)
        {
            StopCoroutine(_unpauseCoroutine);
        }
        _unpauseCoroutine = StartCoroutine(UnpauseProcessesCoroutine(delay));
    }

    private IEnumerator UnpauseProcessesCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
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
    
    public void ProceedGameState(float delay)
    {
        if (_proceedGameStateCoroutine != null)
        {
            StopCoroutine(_proceedGameStateCoroutine);
        }
        _proceedGameStateCoroutine = StartCoroutine(GameStateDelayCoroutine(delay));
    }

    private IEnumerator GameStateDelayCoroutine(float delay)
    {
        PauseProcesses();
        yield return new WaitForSeconds(delay);
        UnpauseProcesses();
        ProceedGameState();
    }

    public void NewRound()
    {
        CurrentMinBid = 0;
        Deck.Reset();
        DiscardPile.Reset();
        RoundCount++;
    }

    public void EndRound()
    {
        Player winningPlayer = GetWinningPlayer();
        CommunityCard = null;
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

    public bool VerifyWinOrLose()
    {
        if (!Players[0].Alive)
        {
            Lose();
            return true;
        }
        if (GetAmountOfAlivePlayers() <= 1)
        {
            Win();
            return true;
        }
        return false;
    }
    
    public void Win()
    {
        SwitchState(new GameWinState());
    }

    public void Lose()
    {
        SwitchState(new GameLoseState());
    }

    public bool IsRoundEndFromFold()
    {
        return GetAmountOfFolded() >= 3;
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
        
        int counter = 0;
        yield return null;
        CurrentMinBid = 0;
        foreach (Player player in Players)
        {
            player.ResetCurrentBidAmount();
        }
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);
        Debug.Log("blind: " + BlindPlayer.Name);
        if (forceBlind)
        {
            ProcessBlind();
            Players.Next();
        }
        
        do
        {
            
            counter++;
            Debug.Log(counter);
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
            CurrentPlayer.UpdateData(DeepCopy());
            SendInputRequest(CurrentPlayer, PlayerRequestType.Bid);
            //Debug.Log("waiting for " + );
            while (_isWaitingForInput)
            {

                yield return null;
            }

            Players.Next();
        } while ((!AreAllPlayersMatchingHighestBet() || counter < Players.Count) && !IsRoundEndFromFold());
        
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);
        Debug.Log("finish");
        UnpauseProcesses(1f);
        if (IsRoundEndFromFold())
        {
            _gameLoopDefinition.SetCurrentIndex(_gameLoopDefinition.Count - 1);
            SwitchState(_gameLoopDefinition[_gameLoopDefinition.Count - 1]);
        }
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
            
            CurrentPlayer.UpdateData(DeepCopy());
            SendInputRequest(CurrentPlayer, PlayerRequestType.Mulligan);
            while (_isWaitingForInput)
            {
                yield return null;
            }

        }
        
        Players.SetCurrentIndex(BlindPlayer.IndexInManager);
        Debug.Log("finish");
        UnpauseProcesses(1f);
    }

    private void ProcessBlind()
    {
        BlindPlayer.BidRequest(BlindBid);
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
        this.BlindBid++;
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
                HandleBidResponse(player, value);
                break;
            case PlayerRequestType.Mulligan:
                HandleMulliganResponse(player, value);
                break;
            //... additional cases as needed ...
        }
        // continue the coroutine
        _isWaitingForInput = false;
    }
    
    public void HandleBidResponse(Player player, int bidValue)
    {
        CurrentMinBid = Mathf.Max(CurrentMinBid, player.CurrentBetAmount);
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

    public int GetAmountOfAlivePlayers()
    {
        return Players.Count(player => player.Alive);
    }

    public int GetAmountOfFolded()
    {
        return Players.Count(player => player.OutOfBetting);
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