using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardGame
{
    static Dictionary<int, string> RankNamePairs = new Dictionary<int, string>()
    {
        { 1, "Null"},
        { 2, "Two"},
        { 3, "Three"},
        { 4, "Four"},
        { 5, "Five" },
        { 6, "Six" },
        { 7, "Seven" },
        { 8, "Eight" },
        { 9, "Nine" },
        { 10, "Ten" },
        { 11, "Jack" },
        { 12, "Queen" },
        { 13, "King" },
        { 14, "Ace" }
    };

    public static string RankToString(int rank)
    {
        if (RankNamePairs.ContainsKey(rank))
        {
            return RankNamePairs[rank];
        } else
        {
            Debug.Log($"Rank to string pair for {rank}, not created. Please visit the CardGame script and add your rank string pair.");
            return string.Empty;
        }
    }
    
    public static int StringToRank(string rankString)
    {
        foreach (var pair in RankNamePairs)
        {
            if (pair.Value == rankString)
            {
                return pair.Key;
            }
        }
        Debug.Log(
            $"Rank string pair for {rankString}, not found. Please visit the CardGame script and add your rank string pair.");
        return -1;
    }
    
    //public static 
}
