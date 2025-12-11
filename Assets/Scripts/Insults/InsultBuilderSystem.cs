using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InsultBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInsultInventory playerInventory;

    [Header("Settings")]
    [SerializeField] private int maxWordsPerInsult = 2;   // current word limit
    [SerializeField] private int maxWordsCap = 5;         // hard cap for upgrades


    private List<InsultWord> currentWords = new List<InsultWord>();
    int indexOfLastWord = 0;


    public IReadOnlyList<InsultWord> CurrentWords => currentWords;
    public int MaxWordsPerInsult => maxWordsPerInsult;
    public PlayerInsultInventory PlayerInventory => playerInventory;

    public bool TryAddWord(InsultWord word)
    {
        if (word == null)
            return false;

        if (currentWords.Count >= maxWordsPerInsult && indexOfLastWord == CurrentWords.Count())
        {
            Debug.Log("Cannot add more words: reached limit.");
            return false;
        }

        if (playerInventory == null || playerInventory.OwnedWords == null)
        {
            Debug.LogWarning("PlayerInsultInventory not set or empty.");
            return false;
        }

        // If you want duplicates, remove this check
        if (currentWords.Contains(word))
        {
            Debug.Log("Cannot add word: word already in the current insult.");
            return false;
        }

        if(indexOfLastWord != currentWords.Count())
        {
            currentWords[indexOfLastWord] = word;
            word.indexInList = indexOfLastWord;
        } else
        {
            currentWords.Add(word);
            word.indexInList = currentWords.Count() - 1;
        }
        indexOfLastWord = currentWords.Count();

        return true;
    }

    public void RemoveWord(InsultWord word)
    {
        if (word == null) return;

        indexOfLastWord = word.indexInList;

        InsultWord emptyInsult = new InsultWord();
        emptyInsult.displayText = "";
        emptyInsult.indexInList = indexOfLastWord;

        currentWords[indexOfLastWord] = emptyInsult;
        word.indexInList = -1;

    }

    public void ClearInsult()
    {
        currentWords.Clear();
        indexOfLastWord = 0;
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
