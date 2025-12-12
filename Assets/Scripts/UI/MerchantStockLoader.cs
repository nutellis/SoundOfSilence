using System.Collections.Generic;
using UnityEngine;

public class MerchantStockLoader : MonoBehaviour
{
    public string resourcesPath = "Assets/Data/InsultWords"; // Resources/Insults
    public bool loadOnStart = true;

    private Merchant merchant;

    private void Awake()
    {
        merchant = GetComponent<Merchant>();
    }

    private void Start()
    {
        if (loadOnStart)
            LoadFromResources();
    }

    [ContextMenu("Load From Resources")]
    public void LoadFromResources()
    {
        if (merchant == null)
        {
            Debug.LogError("MerchantStockLoader: Merchant not found on same GameObject.");
            return;
        }

        InsultWord[] words = Resources.LoadAll<InsultWord>(resourcesPath);
        if (words == null || words.Length == 0)
        {
            Debug.LogWarning($"MerchantStockLoader: No InsultWord found in Resources/{resourcesPath}");
            return;
        }

        // IMPORTANT: Merchant needs a way to accept stock.
        // We call a method you'll add to Merchant: SetStockFromWords(words)
        merchant.SetStockFromWords(words);

        Debug.Log($"MerchantStockLoader: Loaded {words.Length} insults into merchant.");
    }
}
