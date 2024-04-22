public class Card
{
    public CardCollection Owner { get; set; }
    public SuitType Suit { get; private set; }
    public int Rank { get; private set; }
    public string Name { get; private set; }
    public EventTriggerClickable Clickable { get; set; }
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

    public Card SetSuit(SuitType suitType)
    {
        var newCard = new Card(suitType, Rank, Name); 
        return newCard;
    }

    public static Card operator +(Card card, int increment)
    {
        int baseRank = 2;
        int maxRank = 14;
        int newRank = ((card.Rank - baseRank + increment) % (maxRank - baseRank + 1)) + baseRank;
        return new Card(card.Suit, newRank, card.Name);
    }

    public static Card operator -(Card card, int decrement)
    {
        int baseRank = 2;
        int maxRank = 14;
        int newRank = ((card.Rank - baseRank - decrement) % (maxRank - baseRank + 1) + (maxRank - baseRank + 1)) %
            (maxRank - baseRank + 1) + baseRank;
        return new Card(card.Suit, newRank, card.Name);
    }
    
    public static bool operator ==(Card card1, Card card2)
    {
        if (ReferenceEquals(card1, card2))
        {
            return true;
        }
        if (ReferenceEquals(card1, null) || ReferenceEquals(card2, null))
        {
            return false;
        }
        return card1.Suit == card2.Suit && card1.Rank == card2.Rank;
    }
    public static bool operator !=(Card card1, Card card2)
    {
        return !(card1 == card2);
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((Card)obj);
    }
    public bool Equals(Card other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Suit == other.Suit && Rank == other.Rank;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (int)Suit;
            hashCode = (hashCode * 397) ^ Rank;
            return hashCode;
        }
    }

}