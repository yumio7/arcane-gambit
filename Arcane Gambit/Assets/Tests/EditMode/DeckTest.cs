using System.Linq;
using NUnit.Framework;

namespace Tests.EditMode
{
    [TestFixture]
    [TestOf(typeof(Deck))]
    public class DeckTest
    {
        private Deck deck;

        [SetUp]
        public void Setup()
        {
            deck = new Deck();
        }

        [Test]
        public void Constructor_Initializes_With_52_Cards()
        {
            Assert.That(deck.Cards.Count, Is.EqualTo(52));
        }

        [Test]
        public void Constructor_Initializes_With_All_Suits()
        {
            var suitCounts = deck.Cards.GroupBy(c => c.Suit).Select(a => a.Count());
            Assert.That(suitCounts.All(a => a == 13));
        }

        [Test]
        public void Constructor_Initializes_With_All_Ranks()
        {
            var rankCounts = deck.Cards.GroupBy(c => c.Rank).Select(a => a.Count());
            Assert.That(rankCounts.All(a => a == 4));
        }

        [Test]
        public void Shuffle_Changes_Order_Of_Cards()
        {
            var orderedCards = deck.Cards.Select(c => new Card(c.Suit, c.Rank, c.Name)).ToList();
            deck.Shuffle();
            Assert.That(deck.Cards, Is.Not.EqualTo(orderedCards));
        }

        [Test]
        public void DrawCard_Returns_Card_And_Removes_From_Deck()
        {
            var card = deck.DrawCard();
            Assert.That(card, Is.Not.Null);
            Assert.That(deck.Cards, Does.Not.Contain(card.ToString()));
        }

        [Test]
        public void AddCard_Adds_Card_To_Deck()
        {
            var card = new Card(SuitType.Diamond, 1, "TestCard");
            deck.AddCard(card);
            Assert.That(deck.Cards, Does.Contain(card));
        }
    }
}