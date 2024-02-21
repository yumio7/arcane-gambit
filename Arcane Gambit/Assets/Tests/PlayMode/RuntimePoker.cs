using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

public class RuntimePoker
{
    private GameObject _testGameObject;
    private Poker _poker;
    private Player _player1;
    private Player _player2;
    private Player _player3;
    private Player _player4;

    [SetUp]
    public void SetUp()
    {
        
        _testGameObject = new GameObject();
        _poker = _testGameObject.AddComponent<Poker>();
        _player1 = _poker.Players[0];
        _player2 = _poker.Players[1];
        _player3 = _poker.Players[2];
        _player4 = _poker.Players[3];
        
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(_testGameObject);

    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    //[UnityTest]
    public IEnumerator TestPlayerRaise()
    {
        yield return null;
        while (_poker.PokerState.GetType() != typeof(BlindBettingRoundState))
        {
            yield return null;
        }
        Debug.Log(_poker.CurrentPlayer.Name);
        Debug.Log(_player1.BidToMatch);
        _poker.CurrentPlayer.Raise(1);
        Assert.That(_poker.BidPot, Is.EqualTo(1));
        Assert.That(_poker.CurrentPlayer.CurrentBetAmount, Is.EqualTo(1));
        Assert.That(_poker.CurrentPlayer.TotalChips, Is.EqualTo(99));
        yield return null;
        Debug.Log(_poker.CurrentPlayer.Name);
        Debug.Log(_player2.BidToMatch);
        _poker.CurrentPlayer.Raise(1);
        Assert.That(_poker.BidPot, Is.EqualTo(3));
        Assert.That(_poker.CurrentPlayer.CurrentBetAmount, Is.EqualTo(2));
        Assert.That(_poker.CurrentPlayer.TotalChips, Is.EqualTo(98));
    }
    
    [UnityTest]
    public IEnumerator TestPlayerFold()
    {
        yield return null;
        while (_poker.PokerState.GetType() != typeof(BlindBettingRoundState))
        {
            yield return null;
        }
        _poker.CurrentPlayer.Fold();
        yield return null;
        Assert.That(_poker.BidPot, Is.EqualTo(0));
        //Assert.Equals()
    }

    /*[UnityTest]
    public IEnumerator TestProceedBidding()
    {
        yield return null;
        while (_poker.PokerState.GetType() != typeof(BettingRoundState))
        {
            yield return null;
        }
        
    }*/
}
