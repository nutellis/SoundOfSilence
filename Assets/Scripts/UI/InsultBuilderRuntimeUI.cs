using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InsultBuilderRuntimeUI : MonoBehaviour
{
    [Header("References")]
    public InsultBuilder insultBuilder;
    public PlayerInsultInventory inventory;

    [Header("UI Elements")]
    public GameObject panel;
    public List<Button> slotButtons;
    public List<Text> slotLabels;
    public Transform ownedWordsRow;
    public Button fireButton;

    private void Start()
    {
        if (insultBuilder == null)
            insultBuilder = GetComponent<InsultBuilder>();

        if (inventory == null)
            inventory = GetComponent<PlayerInsultInventory>();

        panel.SetActive(false);

        // Slot setup
        for (int i = 0; i < slotButtons.Count; i++)
        {
            int index = i;
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }

        // Fire button
        fireButton.onClick.AddListener(OnFireClicked);

        BuildOwnedWordButtons();
        RefreshSlots();
    }

    private void Update()
    {

    }

    public void StartInsultBuilder()
    {
        panel.SetActive(!panel.activeSelf);
    }

    private void BuildOwnedWordButtons()
    {
        foreach (Transform child in ownedWordsRow)
            Destroy(child.gameObject);

        foreach (var w in inventory.OwnedWords)
        {
            GameObject btnGO = new GameObject(w.displayText);
            btnGO.transform.SetParent(ownedWordsRow);
          //  btnGO.AddComponent<CanvasRenderer>();
            Button btn = btnGO.AddComponent<Button>();
           // Image img = btnGO.AddComponent<Image>();
           // img.color = new Color(1, 1, 1, 0.3f);

            Text label = btnGO.AddComponent<Text>();
            label.text = w.displayText;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.color = Color.black;
            label.alignment = TextAnchor.MiddleCenter;

            btn.onClick.AddListener(() => OnOwnedWordClicked(w));

            // Fit text inside button
            RectTransform rt = btnGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(150, 40);
        }
    }

    private void OnOwnedWordClicked(InsultWord word)
    {
        if (insultBuilder.TryAddWord(word))
            RefreshSlots();
    }

    private void OnSlotClicked(int index)
    {
        if (index >= insultBuilder.CurrentWords.Count)
            return;

        insultBuilder.RemoveWord(insultBuilder.CurrentWords[index]);
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < slotLabels.Count; i++)
        {
            if (i < insultBuilder.CurrentWords.Count)
                slotLabels[i].text = insultBuilder.CurrentWords[i].displayText;
            else
                slotLabels[i].text = "[empty]";
        }
    }

    private void OnFireClicked()
    {
        string insult;
        int dmg;
        int gold;

        if (insultBuilder.BuildInsult(out insult, out dmg, out gold))
        {
            Debug.Log($"FIRED: {insult} | Damage={dmg} | Gold={gold}");
        }
        else
        {
            Debug.LogWarning("No insult built.");
        }

        insultBuilder.ClearInsult();
        RefreshSlots();
        panel.SetActive(false);
    }
}
