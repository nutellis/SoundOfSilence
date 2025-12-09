using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InsultBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInsultInventory playerInventory;

    [Header("Settings")]
    [SerializeField] private int maxWordsPerInsult = 1;   // current word limit
    [SerializeField] private int maxWordsCap = 5;         // hard cap for upgrades

    private List<InsultWord> currentWords = new List<InsultWord>();

    public IReadOnlyList<InsultWord> CurrentWords => currentWords;
    public int MaxWordsPerInsult => maxWordsPerInsult;
    public PlayerInsultInventory PlayerInventory => playerInventory;

    public bool TryAddWord(InsultWord word)
    {
        if (word == null)
            return false;

        if (currentWords.Count >= maxWordsPerInsult)
        {
            Debug.Log("Cannot add more words: reached limit.");
            return false;
        }

        if (playerInventory == null || playerInventory.OwnedWords == null)
        {
            Debug.LogWarning("PlayerInsultInventory not set or empty.");
            return false;
        }

        if (playerInventory.OwnedWords.Contains(word) == false)
        {
            Debug.Log("Cannot add word: player does not own this word.");
            return false;
        }

        // If you want duplicates, remove this check
        if (currentWords.Contains(word))
        {
            Debug.Log("Cannot add word: word already in the current insult.");
            return false;
        }

        currentWords.Add(word);
        return true;
    }

    public void RemoveWord(InsultWord word)
    {
        if (word == null) return;
        currentWords.Remove(word);
    }

    public void ClearInsult()
    {
        currentWords.Clear();
    }

    public string GetInsultText()
    {
        return string.Join(" ", currentWords.Select(w => w.displayText));
    }

    public void IncreaseMaxWords(int amount)
    {
        maxWordsPerInsult += amount;
        if (maxWordsPerInsult < 1)
            maxWordsPerInsult = 1;

        if (maxWordsPerInsult > maxWordsCap)
            maxWordsPerInsult = maxWordsCap;
    }

    // ---- extra helpers for damage & gold ----

    public int GetTotalDamage()
    {
        return currentWords.Sum(w => w.baseDamage);
    }

    public int GetTotalGoldCost()
    {
        return currentWords.Sum(w => w.goldCost);
    }

    public bool BuildInsult(out string text, out int totalDamage, out int totalGold)
    {
        if (currentWords.Count == 0)
        {
            text = null;
            totalDamage = 0;
            totalGold = 0;
            return false;
        }

        text = GetInsultText();
        totalDamage = GetTotalDamage();
        totalGold = GetTotalGoldCost();
        return true;
    }
}
