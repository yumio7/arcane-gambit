using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HandEvaluator
{
    // Hand classification
    private enum HandClass
    {
        high_card,
        one_pair,
        two_pair,
        three_of_a_kind,
        straight,
        flush,
        full_house,
        four_of_a_kind,
        straight_flush
    }

    // Determines if a given histogram matches a 'format' histogram
    private static bool IsHistogramFormat<K, V>(Dictionary<K, V> histogram, List<V> format)
    {
        // Go through our histogram and to check if it's value appears in the format
        foreach (var pair in histogram)
        {
            if (format.Contains(pair.Value))
            {
                // Our histogram value does appear in the format, so remove it
                format.Remove(pair.Value);
            }
        }
        // Our histogram matches the format if it's empty by the time we're done
        return format.Count == 0;
    }

    // Determines if sequnece of numbers increases by 1 in linear order.
    // Assumes sequence is sorted from lowest to highest. 
    private static bool IsSequentialRun(List<int> sequence)
    {
        if (sequence.Count < 2)
        {
            return true;
        }

        for(int i = 1; i < sequence.Count; i++)
        {
            if(sequence[i] - sequence[i - 1] != 1) { return false; }
        }
        return true;
    }

    // Determines a score for a hand of five cards.
    private static int EvaluateHandOfFive(List<Card> cards)
    {
        // Card rank weights
        int[] card_rank_weight = new int[5]{ 1, 15, 210, 2940, 41160};
        // Final score
        int score = 0;
        // Start by calculating the histograms & data for later use
        Dictionary<int, int> rank_histogram = new Dictionary<int, int>();
        Dictionary<SuitType, int> suit_histogram = new Dictionary<SuitType, int>();
        List<int> card_ranks = new List<int>();
        foreach(Card card in cards)
        {
            rank_histogram.TryGetValue(card.Rank, out int rank_val);
            rank_histogram[card.Rank] = rank_val + 1;

            suit_histogram.TryGetValue(card.Suit, out int suit_val);
            suit_histogram[card.Suit] = suit_val + 1;

            card_ranks.Add(card.Rank);
        }
        // The card's ranks in sorted order (smallest -> largest).
        card_ranks.Sort();
        // The 'class' of this hand- ie. nothing vs. pair vs. two-pair etc...
        HandClass h_class = HandClass.high_card;

        // First determine hand 'rank' for initial baseline score.
        if (IsHistogramFormat(rank_histogram, new List<int>{ 1, 1, 1, 2})) { // one pair
            h_class = HandClass.one_pair;
            score = 1000000;
        }
        if (IsHistogramFormat(rank_histogram, new List<int> { 2, 2, 1})) { // two pair
            h_class = HandClass.two_pair;
            score = 2000000;
        }
        if (IsHistogramFormat(rank_histogram, new List<int> { 3, 1, 1})) { // three of a kind
            h_class = HandClass.three_of_a_kind;
            score = 3000000;
        }
        if (IsSequentialRun(card_ranks)) //straight (with high Ace)
        { 
            h_class = HandClass.straight;
            score = 4000000;
        }
        // Special check for straight with Ace as a low card, making a copy 
        // of the hand but replacing all aces with a fake card of rank 1.
        List<Card> handCopy = new List<Card>();
        List<int> card_ranks_new = new List<int>();
        foreach (Card c in cards)
        {
            if (c.Rank == 14)
            {
                handCopy.Add(new Card(c.Suit, 1, "Low Ace"));
            } else
            {
                handCopy.Add(c);
            }
        }
        foreach(Card c in handCopy)
        {
            card_ranks_new.Add(c.Rank);
        }
        card_ranks_new.Sort();
        // We perform the same operation
        if (IsSequentialRun(card_ranks_new)) //straight (with low Ace)
        {
            h_class = HandClass.straight;
            score = 4000000;
            card_ranks = card_ranks_new;
        }
        if (IsHistogramFormat(suit_histogram, new List<int>{ 5})) { //flush
            if (h_class == HandClass.straight)
            {
                h_class = HandClass.straight_flush;
                score = 8000000;
            }
            else
            {
                h_class = HandClass.flush;
                score = 5000000;
            }
        }
        if (IsHistogramFormat(rank_histogram, new List<int>{ 3, 2})) { // full house
            h_class = HandClass.full_house;
            score = 6000000;
        }
        if (IsHistogramFormat(rank_histogram, new List<int>{ 4, 1})) { // four of a kind
            h_class = HandClass.four_of_a_kind;
            score = 7000000;
        }

        // Next, determine individual card scores to compare two hands of the same rank
        if (h_class == HandClass.high_card)
        {
            // Give each card in the hand a weight based on its position 
            for (int i = 0; i < card_ranks.Count; i++)
            {
                score += card_ranks[i] * card_rank_weight[i];
            }
        }
        else if (h_class == HandClass.one_pair)
        {
            // For one pair, we weight the value of the pair as the highest
            foreach (var pair in rank_histogram)
            {
                if (pair.Value == 2)
                {
                    score += pair.Key * card_rank_weight[4];
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    break;
                }
            }
            // Then, we give a score for each of the remaining cards to resolve any pair ties
            for (int i = 0; i < card_ranks.Count; i++)
            {
                score += card_ranks[i] * card_rank_weight[i];
            }
        }
        else if (h_class == HandClass.two_pair)
        {
            // First, we determine which of the two pairs is our highest.
            int highest_pair_rank = 0;
            int second_highest_pair_rank = 0;
            foreach (var pair in rank_histogram)
            {
                if (pair.Value == 2)
                {
                    if (pair.Key > highest_pair_rank)
                    {
                        highest_pair_rank = pair.Key;
                    }
                    else
                    {
                        second_highest_pair_rank = pair.Key;
                    }
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                }
            }
            // Then, add weighted scores
            score += highest_pair_rank * card_rank_weight[4];
            score += second_highest_pair_rank * card_rank_weight[3];
            score += card_ranks[0];
        }
        else if (h_class == HandClass.three_of_a_kind)
        {
            // Three of a kind is simple- weight the value of the three pair
            foreach (var pair in rank_histogram)
            {
                if (pair.Value == 3)
                {
                    score += pair.Key * card_rank_weight[4];
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    break;
                }
            }
            // Then, we give a score for each of the remaining cards to resolve any pair ties
            for (int i = 0; i < card_ranks.Count; i++)
            {
                score += card_ranks[i] * card_rank_weight[i];
            }
        }
        else if (h_class == HandClass.straight)
        {
            // Simply weight it based on the highest card. If two hands share the same highest card,
            // they are guarenteed to be the same (non-flush) straight.
            score += card_ranks[4];
        }
        else if (h_class == HandClass.flush)
        {
            // Same as high card rules. 
            for (int i = 0; i < card_ranks.Count; i++)
            {
                score += card_ranks[i] * card_rank_weight[i];
            }
        }
        else if (h_class == HandClass.full_house)
        {
            // Start by given a weighted score of the triplet
            foreach (var pair in rank_histogram)
            {
                if (pair.Value == 3)
                {
                    score += pair.Key * card_rank_weight[4];
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    break;
                }
            }
            // Then, give a weighted score of the pair
            foreach (var pair in rank_histogram)
            {
                if (pair.Value == 2)
                {
                    score += pair.Key * card_rank_weight[3];
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    break;
                }
            }
        }
        else if (h_class == HandClass.four_of_a_kind)
        {
            // Four of a kind is also simple- weight the value of the four pair
            foreach (var pair in rank_histogram)
            {
                if (pair.Value == 4)
                {
                    score += pair.Key * card_rank_weight[4];
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    card_ranks.Remove(pair.Key);
                    break;
                }
            }
            // Then, we add the score of the final card
            score += card_ranks[0];
        }
        else if (h_class == HandClass.straight_flush)
        {
            // Simply weight it based on the highest card. If two hands share the same highest card,
            // they are guarenteed to be the same straight flush.
            score += card_ranks[4];
        }

        return score;
    }

    // Evaluates and returns a score for a given hand of any size >= 5 cards.
    public static int EvaluateHand(List<Card> cards)
    {
        int start_pointer = 1;
        int temp_pointer = 0;
        int max_score = 0;

        if (cards == null || cards.Count <= 0)
        {
            return max_score;
        }
        
        // Calculate an initial score with the first 5 cards.
        List<Card> original = new List<Card>();
        for (int i = 0; i < 5; i++)
        {
            original.Add(cards[i]);
        }
        max_score = EvaluateHandOfFive(original);

        List<Card> subset = new List<Card>();
        // Until we loop back around, check every possible subset of cards until
        // we find the maximum possible score.
        while (start_pointer != 0)
        {
            subset.Clear();
            temp_pointer = start_pointer;
            for (int i = 0; i < 5; i++)
            {
                subset.Add(cards[temp_pointer]);
                temp_pointer = (temp_pointer + 1 == cards.Count) ? temp_pointer = 0 : temp_pointer + 1;
            }

            int subset_score = EvaluateHandOfFive(subset);
            if (subset_score > max_score)
            {
                max_score = subset_score;
            }

            start_pointer = (start_pointer + 1 == cards.Count) ? 0 : start_pointer + 1;
        }
        return max_score;
    }
}
