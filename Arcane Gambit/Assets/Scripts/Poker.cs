using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public enum GameState
{
    Waiting,
    RoundStart,
    BettingRound,
    MulliganRound,
    RoundEnd
}

public class Poker : MonoBehaviour
{
    public static GameState PokerState { get; private set; } = GameState.RoundStart;

    private List<Player> _players;

    private void Awake()
    {
        
    }

    private void Start()
    {
        //populate players
    }

    void Update()
    {
        switch(PokerState) {
            case GameState.Waiting:
                break;
            case GameState.RoundStart:
                RoundStart();
                break;
            case GameState.BettingRound:
                BettingRound();
                break;
            case GameState.MulliganRound:
                MulliganRound();
                break;
            case GameState.RoundEnd: 
                RoundEnd(); 
                break;
            default: 
                Debug.LogError($"Gamestate : {PokerState} is unrecognized");
                break;
        }
    }

    private void SwitchState(GameState gameState)
    {

    }

    private void RoundStart()
    {

    }

    private void BettingRound()
    {

    }

    private void MulliganRound()
    {

    }
    
    private void RoundEnd()
    {

    }
}
