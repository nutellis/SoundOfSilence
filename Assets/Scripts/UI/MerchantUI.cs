using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public Transform ContentList;       // ScrollRect/Viewport/Content

    [Header("UI - Right Details")]
    public Text detailTitleText;
    public Text detailBodyText;
    public Text detailPriceText;
    public Button buyButton;

    private InsultWord selectedWord;

    private List<ShopItem> merchantList;

    public GameObject buttonPrefab;

    public PlayerInput input;

    private void Start()
    {
        if (merchant == null) merchant = FindObjectOfType<Merchant>();
        if (playerWallet == null) playerWallet = FindObjectOfType<PlayerWallet>();

        if (panel != null) panel.SetActive(false);

        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyClicked);

        //RefreshUI();
        //ClearDetails();

        merchant.interactAction += () => onMerchantInteract();

        merchantList = merchant.insultWordStock;
    }

    private void Update()
    {
    }

    private void onMerchantInteract()
    {
        if(panel != null)
        {
            bool active = !panel.activeSelf;
            panel.SetActive(active);

            if (active)
            {
                Cursor.lockState = CursorLockMode.None;
                input.DeactivateInput();
                RefreshUI();
                ClearDetails();
            } else
            {
                input.ActivateInput();
                Cursor.lockState = CursorLockMode.Locked;
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
        if (merchant == null || ContentList == null || merchantList == null)
            return;

        for (int i = ContentList.childCount - 1; i >= 0; i--)
            Destroy(ContentList.GetChild(i).gameObject);

        foreach (var item in merchantList)
        {
            if (item?.word == null) continue;

            GameObject row = Instantiate(buttonPrefab, ContentList);
            Button btn = row.GetComponent<Button>();
            Text label = row.GetComponentInChildren<Text>();


            int price = item.GetPrice();

            if (label != null)
                label.text = $"{item.word.displayText} - {price}g";

            InsultWord wordCopy = item.word;

            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnSelectWord(wordCopy));
            }
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
}   