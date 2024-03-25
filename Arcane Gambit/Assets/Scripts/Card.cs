public class Card
{
    public CardCollection Owner { get; set; }
    public SuitType Suit { get; private set; }
    public int Rank { get; private set; }
    public string Name { get; private set; }

    public Card(SuitType suit, int rank, string name = "")
    {
        Suit = suit;
        Rank = rank;
        Name = name;
        if (Name == "")
        {
            Name = FormattedName();
        }
    }

    public Card(Card other)
    {
        Suit = other.Suit;
        Rank = other.Rank;
        Name = other.Name;
    }

    public string FormattedName()
    {
        return $"{CardGame.RankToString(Rank)} of {Suit}s";
    }

    public override string ToString()
    {
        return FormattedName();
    }

}