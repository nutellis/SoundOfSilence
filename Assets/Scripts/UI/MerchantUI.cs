using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    [Header("References")]
    public Merchant merchant;
    public PlayerWallet playerWallet;

    [Header("UI Objects")]
    public GameObject panel;                    // MerchantPanel
    public TextMeshProUGUI goldText;            // shows "Gold: X"
    public Transform wordListParent;            // parent for shop items (Vertical Layout Group)
    public GameObject wordEntryPrefab;          // prefab for each word row (Button + TMP text)
    public Button slotUpgradeButton;            // "Buy Slot Upgrade" button
    public TextMeshProUGUI slotUpgradePriceText; // shows price for slot upgrade

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.M;       // open/close shop with M

    private void Start()
    {
        if (merchant == null)
            merchant = FindObjectOfType<Merchant>();

        if (playerWallet == null)
            playerWallet = FindObjectOfType<PlayerWallet>();

        if (panel != null)
            panel.SetActive(false);

        if (slotUpgradeButton != null)
            slotUpgradeButton.onClick.AddListener(OnSlotUpgradeClicked);

        RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && panel != null)
        {
            bool active = !panel.activeSelf;
            panel.SetActive(active);

            if (active)
                RefreshUI();
        }
    }

    private void RefreshUI()
    {
        if (playerWallet != null && goldText != null)
            goldText.text = $"Gold: {playerWallet.CurrentGold}";

        if (merchant != null && slotUpgradePriceText != null)
            slotUpgradePriceText.text = $"Buy slot (+1) - {GetSlotUpgradePrice()}g";

        BuildWordList();
    }

    private int GetSlotUpgradePrice()
    {
        // we exposed slotUpgradePrice in Merchant, but not public – easiest is:
        // use reflection OR just hardcode same value here.
        // For now, we assume 20g like in Merchant.cs
        return 20;
    }

    private void BuildWordList()
    {
        if (merchant == null || wordListParent == null || wordEntryPrefab == null)
            return;

        // Clear old entries
        for (int i = wordListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(wordListParent.GetChild(i).gameObject);
        }

        IReadOnlyList<Merchant.ShopItem> stock = merchant.GetStock();
        foreach (var item in stock)
        {
            if (item == null || item.word == null)
                continue;

            GameObject entryGO = Instantiate(wordEntryPrefab, wordListParent);
            Button btn = entryGO.GetComponent<Button>();

            TextMeshProUGUI label = entryGO.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                int price = item.GetPrice();
                label.text = $"{item.word.displayText} - {price}g";
            }

            InsultWord wordCopy = item.word;
            btn.onClick.AddListener(() => OnBuyWordClicked(wordCopy));
        }
    }

    private void OnBuyWordClicked(InsultWord word)
    {
        if (merchant == null)
            return;

        bool success = merchant.TryBuyInsultWord(word);
        if (success)
        {
            Debug.Log($"MerchantUI: Bought word {word.displayText}");
            RefreshUI();
        }
        else
        {
            Debug.Log("MerchantUI: Could not buy word.");
            RefreshUI();
        }
    }

    private void OnSlotUpgradeClicked()
    {
        if (merchant == null)
            return;

        bool success = merchant.TryBuySlotUpgrade();
        if (success)
        {
            Debug.Log("MerchantUI: Bought slot upgrade.");
            RefreshUI();
        }
        else
        {
            Debug.Log("MerchantUI: Could not buy slot upgrade.");
            RefreshUI();
        }
    }
}
