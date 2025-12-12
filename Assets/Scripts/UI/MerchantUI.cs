using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    [Header("References")]
    public Merchant merchant;
    public PlayerWallet playerWallet;

    [Header("UI - Root")]
    public GameObject panel;     // Merchant UI root
    public Text goldText;        // "Gold: X"

    [Header("UI - Left List")]
    public Transform listParent;       // ScrollRect/Viewport/Content
    public GameObject listEntryPrefab; // Button + child Text

    [Header("UI - Right Details")]
    public Text detailTitleText;
    public Text detailBodyText;
    public Text detailPriceText;
    public Button buyButton;

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.M;

    [Header("Interaction")]
    public string playerTag = "Player";

    private bool canInteract;
    private InsultWord selectedWord;

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
        if (playerWallet != null && goldText != null)
            goldText.text = $"Gold: {playerWallet.CurrentGold}";

        BuildList();
    }

    private void BuildList()
    {
        if (merchant == null || listParent == null || listEntryPrefab == null)
            return;

        for (int i = listParent.childCount - 1; i >= 0; i--)
            Destroy(listParent.GetChild(i).gameObject);

        IReadOnlyList<Merchant.ShopItem> stock = merchant.GetStock();

        foreach (var item in stock)
        {
            if (item?.word == null) continue;

            GameObject row = Instantiate(listEntryPrefab, listParent);

            Button btn = row.GetComponent<Button>();
            Text label = row.GetComponentInChildren<Text>();

            int price = item.GetPrice();

            if (label != null)
                label.text = $"{item.word.displayText} - {price}g";

            InsultWord wordCopy = item.word;

            if (btn != null)
                btn.onClick.AddListener(() => OnSelectWord(wordCopy));
        }
    }

    private void OnSelectWord(InsultWord word)
    {
        selectedWord = word;

        if (detailTitleText != null)
            detailTitleText.text = word.displayText;

        if (detailBodyText != null)
        {
            detailBodyText.text =
                $"Damage: {word.baseDamage}\n" +
                $"Cost: {word.goldCost}g";
        }

        if (detailPriceText != null && merchant != null)
            detailPriceText.text = $"Price: {merchant.GetPriceForWord(word)}g";

        if (buyButton != null)
            buyButton.interactable = true;
    }

    private void ClearDetails()
    {
        selectedWord = null;

        if (detailTitleText != null) detailTitleText.text = "Choose an insult";
        if (detailBodyText != null) detailBodyText.text = "";
        if (detailPriceText != null) detailPriceText.text = "";

        if (buyButton != null)
            buyButton.interactable = false;
    }

    private void OnBuyClicked()
    {
        if (merchant == null || selectedWord == null) return;

        bool success = merchant.TryBuyInsultWord(selectedWord);

        RefreshUI();

        if (success)
            ClearDetails();
    }

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
            if (panel != null) panel.SetActive(false);
        }
    }
}
