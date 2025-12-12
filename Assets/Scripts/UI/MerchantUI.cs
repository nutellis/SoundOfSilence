using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    [Header("References")]
    public Merchant merchant;
    public PlayerWallet playerWallet;

    [Header("UI - Root")]
    public GameObject panel;     // whole merchant UI root (MerchantPanel)
    public Text goldText;        // Legacy Text: "Gold: X"

    [Header("UI - Left List")]
    public Transform listParent;     // Content object inside ScrollRect
    public GameObject listEntryPrefab; // prefab: Button + (child) Legacy Text

    [Header("UI - Right Details")]
    public Text detailTitleText;     // Legacy Text (e.g., insult name)
    public Text detailBodyText;      // Legacy Text (tags/desc etc.)
    public Text detailPriceText;     // Legacy Text (e.g., "Price: 10g")
    public Button buyButton;         // Buy button

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.M;

    [Header("Interaction")]
    public string playerTag = "Player";

    private bool canInteract;
    private InsultWord selectedWord;
    private int selectedPrice;

    private void Start()
    {
        if (merchant == null) merchant = FindObjectOfType<Merchant>();
        if (playerWallet == null) playerWallet = FindObjectOfType<PlayerWallet>();

        if (panel != null) panel.SetActive(false);

        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyClicked);

        RefreshUI();
        ClearDetails();
    }

    private void Update()
    {
        if (!canInteract || panel == null) return;

        if (Input.GetKeyDown(toggleKey))
        {
            bool active = !panel.activeSelf;
            panel.SetActive(active);

            if (active)
            {
                RefreshUI();
                ClearDetails();
            }
        }
    }

    private void RefreshUI()
    {
        // Gold
        if (playerWallet != null && goldText != null)
            goldText.text = $"Gold: {playerWallet.CurrentGold}";

        // Rebuild list
        BuildList();
    }

    private void BuildList()
    {
        if (merchant == null || listParent == null || listEntryPrefab == null)
            return;

        // Clear old entries
        for (int i = listParent.childCount - 1; i >= 0; i--)
            Destroy(listParent.GetChild(i).gameObject);

        IReadOnlyList<Merchant.ShopItem> stock = merchant.GetStock();
        foreach (var item in stock)
        {
            if (item?.word == null) continue;

            GameObject entryGO = Instantiate(listEntryPrefab, listParent);

            Button btn = entryGO.GetComponent<Button>();
            Text label = entryGO.GetComponentInChildren<Text>(); // Legacy Text in children

            int price = item.GetPrice();
            if (label != null)
                label.text = $"{item.word.displayText} - {price}g";

            InsultWord wordCopy = item.word;
            int priceCopy = price;

            if (btn != null)
                btn.onClick.AddListener(() => OnSelectWord(wordCopy, priceCopy));
        }
    }

    private void OnSelectWord(InsultWord word, int price)
    {
        selectedWord = word;
        selectedPrice = price;

        if (detailTitleText != null)
            detailTitleText.text = word != null ? word.displayText : "—";

        // Build a readable details string for your current InsultWord fields
        // (Adjust these lines to match your InsultWord class exactly)
        if (detailBodyText != null)
        {
            string tags = (word != null && word.tags != null)
                ? string.Join(", ", word.tags)
                : "No tags";

            detailBodyText.text =
                $"Tags: {tags}\n" +
                $"Damage: {word.baseDamage}\n" +
                $"Gold: {word.goldCost}\n";
        }


        if (detailPriceText != null)
            detailPriceText.text = $"Price: {selectedPrice}g";

        if (buyButton != null)
            buyButton.interactable = (selectedWord != null);
    }

    private void ClearDetails()
    {
        selectedWord = null;
        selectedPrice = 0;

        if (detailTitleText != null) detailTitleText.text = "Choose an insult";
        if (detailBodyText != null) detailBodyText.text = "";
        if (detailPriceText != null) detailPriceText.text = "";

        if (buyButton != null)
            buyButton.interactable = false;
    }

    private void OnBuyClicked()
    {
        if (merchant == null || selectedWord == null)
            return;

        bool success = merchant.TryBuyInsultWord(selectedWord);
        if (success)
        {
            Debug.Log($"MerchantUI: Bought {selectedWord.displayText}");
            RefreshUI();
            ClearDetails(); // optional: force reselect after buying
        }
        else
        {
            Debug.Log("MerchantUI: Could not buy (not enough gold / already owned / etc.)");
            RefreshUI();
        }
    }

    // Use TRIGGERS (recommended)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            canInteract = false;

            // auto close UI when leaving merchant
            if (panel != null) panel.SetActive(false);
        }
    }
}
