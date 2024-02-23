using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace Tests.EditMode
{
    [TestFixture]
    [TestOf(typeof(Poker))]
    public class PokerTest
    {
        /*private Deck deck;
        private Hand hand1, hand2, hand3, hand4;

        [SetUp]
        public void SetUp()
        {
            deck = new Deck();

            // Assuming Hand and Card classes and a method in Deck to draw a card
            hand1 = new Hand(5); //straight flush
            hand2 = new Hand(5); //flush
            hand3 = new Hand(5); //two pair
            hand4 = new Hand(5); //two pair

            hand1.AddCard(new List<Card>()
            {
                new Card(SuitType.Club, 3),
                new Card(SuitType.Club, 4),
                new Card(SuitType.Club, 5),
                new Card(SuitType.Club, 6),
                new Card(SuitType.Club, 7)
            });
            
            hand2.AddCard(new List<Card>()
            {
                new Card(SuitType.Club, 2),
                new Card(SuitType.Club, 4),
                new Card(SuitType.Club, 5),
                new Card(SuitType.Club, 9),
                new Card(SuitType.Club, 7)
            });

            hand3.AddCard(new List<Card>()
            {
                new Card(SuitType.Club, 2),
                new Card(SuitType.Diamond, 2),
                new Card(SuitType.Club, 5),
                new Card(SuitType.Club, 9),
                new Card(SuitType.Club, 7)
            });
            
            hand4.AddCard(new List<Card>()
            {
                new Card(SuitType.Club, 2),
                new Card(SuitType.Diamond, 2),
                new Card(SuitType.Club, 5),
                new Card(SuitType.Club, 10),
                new Card(SuitType.Club, 7)
            });
        }
        
        [Test]
        public void Test_GetWinningPlayer()
    {
        Poker poker = new Poker();
        poker.Players[0].Hand.AddCard(hand1.Cards);
        poker.Players[1].Hand.AddCard(hand2.Cards);
        poker.Players[2].Hand.AddCard(hand3.Cards);
        poker.Players[3].Hand.AddCard(hand4.Cards);
        Player winningPlayer = poker.GetWinningPlayer();
        Assert.AreEqual(poker.Players[0], winningPlayer);
    }*/
    }
}