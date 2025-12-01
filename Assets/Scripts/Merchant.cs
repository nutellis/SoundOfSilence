using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [Header("Shop Stock")]
    [SerializeField] private List<InsultWord> stock = new();   // words the merchant sells

    [Header("References")]
    [SerializeField] private PlayerInsultInventory playerInventory;

    [Header("TEMP: Gold for testing")]
    [SerializeField] private int playerGold = 20;              // later connect to real currency system

    public int PlayerGold => playerGold;

    public bool TryBuyWord(InsultWord word)
    {
        if (word == null) return false;
        if (!stock.Contains(word)) return false;
        int cost = word.goldCost;

        if (playerGold < cost)
        {
            Debug.Log("Not enough gold to buy: " + word.displayText);
            return false;
        }

        bool added = playerInventory.AddWord(word);
        if (!added)
        {
            Debug.Log("Could not add word to inventory (maybe duplicate?): " + word.displayText);
            return false;
        }

        playerGold -= cost;
        Debug.Log($"Bought '{word.displayText}' for {cost} gold. Remaining: {playerGold}");
        RemoveWordFromStock(word);

        return true;
    }

    public IReadOnlyList<InsultWord> GetStock()
    {
        return stock;
    }

    // If We want to remove a word from stock after purchase
    public void RemoveWordFromStock(InsultWord word)
    {
        stock.Remove(word);
    }


    private void Start()
    {
        // TEMP TEST: try to buy the first word in stock when the game starts.
        if (stock.Count > 0)
        {
            TryBuyWord(stock[0]);
        }
    }
}
