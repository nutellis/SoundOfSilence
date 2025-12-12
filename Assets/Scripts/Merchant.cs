using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ShopItem
{
    public InsultWord word;
    public int priceOverride = -1;  // if < 0, use word.goldCost

    public int GetPrice()
    {
        if (word == null) return 0;
        return priceOverride > 0 ? priceOverride : word.goldCost;
    }
}

public class Merchant : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInsultInventory playerInventory;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private InsultBuilder insultBuilder;

    [Header("Shop Stock")]
    [SerializeField] public List<ShopItem> insultWordStock = new();

    [Header("Upgrades")]
    [SerializeField] private int slotUpgradePrice = 20; // gold cost per extra slot

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.M;
    public Action interactAction;

    bool canInteract;

    private void Awake()
    {
        // Try auto-wire if not set
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInsultInventory>();

        if (playerWallet == null)
            playerWallet = FindObjectOfType<PlayerWallet>();

        if (insultBuilder == null)
            insultBuilder = FindObjectOfType<InsultBuilder>();
    }

    private void Update()
    {
        if(canInteract)
        {
            if (Input.GetKeyDown(toggleKey))
            {
                interactAction();
            }
        }

    }

    // --------------- PUBLIC API ---------------

    /// <summary>
    /// Try to buy a specific insult word from the merchant.
    /// Returns true on success, false if cannot (no gold / already own / null).
    /// </summary>
    public bool TryBuyInsultWord(InsultWord word)
    {
        if (word == null)
        {
            Debug.LogWarning("Merchant: Tried to buy null word.");
            return false;
        }

        if (playerInventory == null || playerWallet == null)
        {
            Debug.LogError("Merchant: Missing playerInventory or playerWallet reference.");
            return false;
        }

        // Already owned?
        if (playerInventory.HasWord(word))
        {
            Debug.Log("Merchant: Player already owns this word.");
            return false;
        }

        // Find this word in stock to get price
        ShopItem item = insultWordStock.Find(i => i.word == word);
        if (item == null)
        {
            Debug.LogWarning("Merchant: That word is not in my stock.");
            return false;
        }

        int price = item.GetPrice();

        if (!playerWallet.CanAfford(price))
        {
            Debug.Log("Merchant: Player cannot afford this word.");
            return false;
        }

        // Spend & add to inventory
        bool spent = playerWallet.Spend(price);
        if (!spent)
        {
            Debug.LogError("Merchant: Spend failed even though CanAfford was true.");
            return false;
        }

        bool added = playerInventory.AddWord(word);
        if (!added)
        {
            Debug.LogWarning("Merchant: Could not add word to inventory (maybe duplicate blocked?). Refunding.");
            playerWallet.AddGold(price); // refund
            return false;
        }

        Debug.Log($"Merchant: Player bought word '{word.displayText}' for {price} gold.");
        return true;
    }

    /// <summary>
    /// Try to buy a +1 insult slot upgrade for the player.
    /// </summary>
    public bool TryBuySlotUpgrade()
    {
        if (insultBuilder == null || playerWallet == null)
        {
            Debug.LogError("Merchant: Missing insultBuilder or playerWallet reference.");
            return false;
        }

        if (!playerWallet.CanAfford(slotUpgradePrice))
        {
            Debug.Log("Merchant: Player cannot afford slot upgrade.");
            return false;
        }

        bool spent = playerWallet.Spend(slotUpgradePrice);
        if (!spent)
        {
            Debug.LogError("Merchant: Spend failed even though CanAfford was true (slot upgrade).");
            return false;
        }

        insultBuilder.IncreaseMaxWords(1);

        Debug.Log($"Merchant: Player bought +1 insult slot for {slotUpgradePrice} gold. New max = {insultBuilder.MaxWordsPerInsult}");
        return true;
    }

    // --------------- HELPERS FOR UI ---------------

    public IReadOnlyList<ShopItem> GetStock() => insultWordStock;

    public int GetPriceForWord(InsultWord word)
    {
        if (word == null) return 0;
        ShopItem item = insultWordStock.Find(i => i.word == word);
        return item != null ? item.GetPrice() : 0;
    }

    public void SetStockFromWords(IEnumerable<InsultWord> words, bool clearFirst = true)
    {
        if (words == null) return;

        if (clearFirst) insultWordStock.Clear();

        foreach (var w in words)
        {
            if (w == null) continue;

            insultWordStock.Add(new ShopItem
            {
                word = w,
                priceOverride = -1 // use word.goldCost
            });
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}
