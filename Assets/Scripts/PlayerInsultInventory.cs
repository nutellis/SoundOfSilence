using System.Collections.Generic;
using UnityEngine;

public class PlayerInsultInventory : MonoBehaviour
{
    [SerializeField] private List<InsultWord> ownedWords = new();

    public IReadOnlyList<InsultWord> OwnedWords => ownedWords;

    public bool AddWord(InsultWord word)
    {
        if (word == null) return false;

        // TODO (for you): decide if duplicates are allowed.
        if (ownedWords.Contains(word))
        {
            // For now we block duplicates.
            return false;
        }

        ownedWords.Add(word);
        return true;
    }
}
