using UnityEngine;

public class MerchantStockLoader : MonoBehaviour
{
    [Tooltip("Looks in Assets/Resources/<Assets/Data/InsultWords>")]
    public string resourcesPath = "Assets/Data/InsultWords";
    public bool loadOnStart = true;
    public bool clearExistingStock = true;

    private Merchant merchant;

    private void Awake()
    {
        merchant = GetComponent<Merchant>();
    }

    private void Start()
    {
        if (loadOnStart) Load();
    }

    [ContextMenu("Load Stock Now")]
    public void Load()
    {
        if (merchant == null)
        {
            Debug.LogError("MerchantStockLoader: No Merchant found on this GameObject.");
            return;
        }

        InsultWord[] words = Resources.LoadAll<InsultWord>(resourcesPath);

        if (words == null || words.Length == 0)
        {
            Debug.LogWarning($"MerchantStockLoader: No InsultWord found at Resources/{resourcesPath}");
            return;
        }

        merchant.SetStockFromWords(words, clearExistingStock);
        Debug.Log($"MerchantStockLoader: Loaded {words.Length} insults into merchant stock.");
    }
}
