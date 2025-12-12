using System.Collections.Generic;
using UnityEngine;

public class PlayerInsultInventory : MonoBehaviour
{
    [Header("Owned Insult Words")]
    [SerializeField] private List<InsultWord> ownedWords = new();

    // Read-only view from outside
    public IReadOnlyList<InsultWord> OwnedWords => ownedWords;

    /// <summary>
    /// Adds a word to the inventory if it isn't already there.
    /// Returns true if added, false if it was null or already present.
    /// </summary>
    public bool AddWord(InsultWord word)
    {
        if (word == null)
            return false;

        // Block duplicates for now
        if (ownedWords.Contains(word))
            return false;

        ownedWords.Add(word);
        return true;
    }

    /// <summary>
    /// Removes a word from the inventory.
    /// Returns true if it was removed, false if it wasn't found.
    /// </summary>
    public bool RemoveWord(InsultWord word)
    {
        if (word == null)
            return false;

        return ownedWords.Remove(word);
    }

    /// <summary>
    /// Check if the player owns a specific word.
    /// </summary>
    public bool HasWord(InsultWord word)
    {
        if (word == null)
            return false;

        return ownedWords.Contains(word);
    }

    /// <summary>
    /// Completely clears the inventory (use carefully).
    /// </summary>
    public void ClearAllWords()
    {
        ownedWords.Clear();
    }
}
