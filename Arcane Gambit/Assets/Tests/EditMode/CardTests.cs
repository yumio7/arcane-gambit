using NUnit.Framework;

namespace Tests.EditMode
{
    [TestFixture]
    public class CardTests
    {

        [Test]
        public void CardGameRankToString()
        {
            Assert.AreEqual(CardGame.RankToString(2), "Two");
            Assert.AreEqual(CardGame.RankToString(14), "Ace");
            Assert.AreEqual(CardGame.RankToString(7), "Seven");
            Assert.AreEqual(CardGame.RankToString(13), "King");
        }
    }
}

