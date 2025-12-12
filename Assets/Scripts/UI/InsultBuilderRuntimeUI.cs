using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

public class InsultBuilderRuntimeUI : MonoBehaviour
{
    [Header("References")]
    public InsultBuilder insultBuilder;
    public PlayerInsultInventory inventory;

    [Header("UI Elements")]
    public GameObject panel;
    public Transform currentInsult;
    public Transform ownedWordsRow;
    public Button fireButton;

    GameObject bossObject;

    public GameObject buttonPrefab;

    PlayerInput input;

    private void Start()
    {
        if (insultBuilder == null)
            insultBuilder = GetComponent<InsultBuilder>();

        if (inventory == null)
            inventory = GetComponent<PlayerInsultInventory>();

        panel.SetActive(false);

        // Fire button
        fireButton.onClick.AddListener(OnFireClicked);

        BuildOwnedWordButtons();
        RefreshSlots();
    }

    private void Update()
    {

    }

    public void StartInsultBuilder(PlayerInput inInput)
    {
        Cursor.lockState = CursorLockMode.None;

        panel.SetActive(true);

        input = inInput;

        Time.timeScale = 0.0f;
    }

    private void BuildOwnedWordButtons()
    {
        foreach (Transform child in ownedWordsRow)
            Destroy(child.gameObject);

        foreach (var w in inventory.OwnedWords)
        {
            GameObject row = Instantiate(buttonPrefab, ownedWordsRow);
            Button btn = row.GetComponent<Button>();
            Text label = row.GetComponentInChildren<Text>();

            label.text = w.displayText;

            btn.onClick.AddListener(() => OnOwnedWordClicked(w));
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
        foreach (Transform child in currentInsult)
            Destroy(child.gameObject);

        foreach (var w in insultBuilder.CurrentWords)
        {

            GameObject row = Instantiate(buttonPrefab, currentInsult);
            Button btn = row.GetComponent<Button>();
            Text label = row.GetComponentInChildren<Text>();

            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(128, rt.sizeDelta.y);

            label.text = w.displayText;
            
            btn.onClick.AddListener(() => RemoveWordFromList(w));

        }
    }

    private void RemoveWordFromList(InsultWord word)
    {
        insultBuilder.RemoveWord(word);
        RefreshSlots();
    }



    private void OnFireClicked()
    {
        string insult;
        int dmg;
        int gold;

        if (insultBuilder.BuildInsult(out insult, out dmg, out gold))
        {
            Debug.Log($"FIRED: {insult} | Damage={dmg} | Gold={gold}");

            TtsHelper sound = GetComponent<TtsHelper>();
            if (sound)
            {
               StartCoroutine(sound.PlayInsult(insult));
            }

            bossObject = GameObject.FindWithTag("Boss");
            if(bossObject)
            {
                bossObject.GetComponent<MinionHealth>().TakeDamage(dmg);

            } else
            {
                Debug.Log("No boss to insult");
            }
            insultBuilder.ClearInsult();
            RefreshSlots();
            panel.SetActive(false);
            input.ActivateInput();
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Debug.LogWarning("No insult built.");
        }        
    }
}
