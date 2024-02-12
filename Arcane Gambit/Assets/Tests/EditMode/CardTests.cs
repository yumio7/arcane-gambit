using NUnit.Framework;

namespace Tests.EditMode
{
    [TestFixture]
    public class CardTests
    {

        [Test]
        public void CardGameRankToString()
        {
            Assert.AreEqual(CardGame.RankToString(1), "Ace");
            Assert.AreEqual(CardGame.RankToString(14), string.Empty);
            Assert.AreEqual(CardGame.RankToString(7), "Seven");
            Assert.AreEqual(CardGame.RankToString(13), "King");
        }
    }
}

