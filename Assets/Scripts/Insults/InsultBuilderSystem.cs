using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class InsultBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInsultInventory playerInventory;

    [Header("Settings")]
    [SerializeField] private int maxWordsPerInsult = 1;   // your "word holders"

    private List<InsultWord> currentWords = new List<InsultWord>();

    public IReadOnlyList<InsultWord> CurrentWords => currentWords;
    public int MaxWordsPerInsult => maxWordsPerInsult;

    public bool TryAddWord(InsultWord word)
    {
        if (word == null)
        {
            return false;
        }

        if (currentWords.Count >= maxWordsPerInsult)
        {
            Debug.Log("Cannot add more words: reached limit.");
            return false;
        }

        if (playerInventory.OwnedWords.Contains(word) == false)
        {
            Debug.Log("Cannot add word: player does not own this word.");
            return false;
        }

        // If you want to have duplicate words in the same insult, remove this check
        if (currentWords.Contains(word) == true)
        {
            Debug.Log("Cannot add word: word already in the current insult.");
            return false;
        }

        currentWords.Add(word);
        return true;
    }

    // Call this to remove a specific word from the insult
    public void RemoveWord(InsultWord word)
    {
        if (word == null) return;
        currentWords.Remove(word);
    }

    // Call this when you want to start building a fresh insult
    public void ClearInsult()
    {
        currentWords.Clear();
    }

    // This returns the final insult text,
    public string GetInsultText()
    {
        return string.Join(" ", currentWords.Select(w => w.displayText));
    }

    // Later you can call this from merchant or upgrades
    // There is no Maximum on the words
    public void IncreaseMaxWords(int amount)
    {
        maxWordsPerInsult += amount;
        if (maxWordsPerInsult < 1)
            maxWordsPerInsult = 1;
    }
}
