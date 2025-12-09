using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InsultBuilderUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InsultBuilder insultBuilder;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.R;

    private Canvas canvas;
    private GameObject panel;
    private GameObject slotsRow;
    private GameObject wordsRow;
    private Button fireButton;

    private const int MaxSlotsUI = 5;

    private readonly List<Button> slotButtons = new List<Button>();
    private readonly List<Text> slotLabels = new List<Text>();
    private readonly List<Button> wordButtons = new List<Button>();

    private void Awake()
    {
        if (insultBuilder == null)
            insultBuilder = GetComponent<InsultBuilder>();

        EnsureEventSystem();
        CreateCanvasAndPanel();
        CreateSlotsRow();
        CreateWordsRow();
        CreateFireButton();
        BuildWordButtonsFromInventory();

        panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        if (panel == null) return;
        panel.SetActive(!panel.activeSelf);
    }

    // ------------- UI Creation -------------

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    private void CreateCanvasAndPanel()
    {
        GameObject canvasGO = new GameObject("InsultBuilderCanvas");
        canvasGO.layer = LayerMask.NameToLayer("UI");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        DontDestroyOnLoad(canvasGO);

        panel = new GameObject("InsultBuilderPanel");
        panel.transform.SetParent(canvasGO.transform, false);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.7f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(800, 400);
        rect.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup vLayout = panel.AddComponent<VerticalLayoutGroup>();
        vLayout.childAlignment = TextAnchor.MiddleCenter;
        vLayout.spacing = 10f;
        vLayout.childControlWidth = true;
        vLayout.childControlHeight = true;
        vLayout.childForceExpandHeight = false;
        vLayout.childForceExpandWidth = true;

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void CreateSlotsRow()
    {
        slotsRow = new GameObject("SlotsRow");
        slotsRow.transform.SetParent(panel.transform, false);

        HorizontalLayoutGroup hLayout = slotsRow.AddComponent<HorizontalLayoutGroup>();
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.spacing = 10f;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandHeight = true;
        hLayout.childForceExpandWidth = true;

        RectTransform rect = slotsRow.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(760, 100);

        for (int i = 0; i < MaxSlotsUI; i++)
        {
            GameObject slotGO = new GameObject($"Slot{i + 1}");
            slotGO.transform.SetParent(slotsRow.transform, false);

            Image bg = slotGO.AddComponent<Image>();
            bg.color = new Color(1f, 1f, 1f, 0.15f);

            Button btn = slotGO.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = new Color(1f, 1f, 1f, 0.15f);
            cb.highlightedColor = new Color(1f, 1f, 1f, 0.35f);
            cb.pressedColor = new Color(1f, 1f, 1f, 0.55f);
            btn.colors = cb;

            GameObject labelGO = new GameObject("Label");
            labelGO.transform.SetParent(slotGO.transform, false);

            Text label = labelGO.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = 20;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = "[empty]";

            RectTransform lRect = labelGO.GetComponent<RectTransform>();
            lRect.anchorMin = Vector2.zero;
            lRect.anchorMax = Vector2.one;
            lRect.offsetMin = Vector2.zero;
            lRect.offsetMax = Vector2.zero;

            int index = i;
            btn.onClick.AddListener(() => OnSlotClicked(index));

            slotButtons.Add(btn);
            slotLabels.Add(label);
        }

        RefreshSlotsFromBuilder();
    }

    private void CreateWordsRow()
    {
        wordsRow = new GameObject("WordsRow");
        wordsRow.transform.SetParent(panel.transform, false);

        HorizontalLayoutGroup hLayout = wordsRow.AddComponent<HorizontalLayoutGroup>();
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.spacing = 5f;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandHeight = true;
        hLayout.childForceExpandWidth = false;

        RectTransform rect = wordsRow.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(760, 200);
    }

    private void CreateFireButton()
    {
        GameObject fireGO = new GameObject("FireButton");
        fireGO.transform.SetParent(panel.transform, false);

        Image img = fireGO.AddComponent<Image>();
        img.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);

        fireButton = fireGO.AddComponent<Button>();
        ColorBlock cb = fireButton.colors;
        cb.normalColor = new Color(0.8f, 0.2f, 0.2f, 0.9f);
        cb.highlightedColor = new Color(1f, 0.3f, 0.3f, 0.95f);
        cb.pressedColor = new Color(0.6f, 0.1f, 0.1f, 0.9f);
        fireButton.colors = cb;

        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(fireGO.transform, false);

        Text label = labelGO.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 24;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = "FIRE INSULT";

        RectTransform lRect = labelGO.GetComponent<RectTransform>();
        lRect.anchorMin = Vector2.zero;
        lRect.anchorMax = Vector2.one;
        lRect.offsetMin = Vector2.zero;
        lRect.offsetMax = Vector2.zero;

        RectTransform rect = fireGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 60);

        fireButton.onClick.AddListener(OnFireClicked);
    }

    private void BuildWordButtonsFromInventory()
    {
        if (insultBuilder == null || insultBuilder.PlayerInventory == null)
            return;

        var owned = insultBuilder.PlayerInventory.OwnedWords;
        if (owned == null) return;

        foreach (var w in owned)
        {
            if (w == null) continue;

            GameObject btnGO = new GameObject($"Word_{w.displayText}");
            btnGO.transform.SetParent(wordsRow.transform, false);

            Image img = btnGO.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.25f);

            Button btn = btnGO.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = new Color(1f, 1f, 1f, 0.25f);
            cb.highlightedColor = new Color(1f, 1f, 1f, 0.45f);
            cb.pressedColor = new Color(1f, 1f, 1f, 0.65f);
            btn.colors = cb;

            GameObject labelGO = new GameObject("Label");
            labelGO.transform.SetParent(btnGO.transform, false);

            Text label = labelGO.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = 18;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = w.displayText;

            RectTransform lRect = labelGO.GetComponent<RectTransform>();
            lRect.anchorMin = Vector2.zero;
            lRect.anchorMax = Vector2.one;
            lRect.offsetMin = Vector2.zero;
            lRect.offsetMax = Vector2.zero;

            InsultWord capturedWord = w;
            btn.onClick.AddListener(() => OnWordButtonClicked(capturedWord));

            wordButtons.Add(btn);
        }
    }

    // ------------- Interaction -------------

    private void OnWordButtonClicked(InsultWord word)
    {
        if (insultBuilder == null) return;

        bool added = insultBuilder.TryAddWord(word);
        if (added)
            RefreshSlotsFromBuilder();
    }

    private void OnSlotClicked(int index)
    {
        if (insultBuilder == null) return;

        var words = insultBuilder.CurrentWords;
        if (index < 0 || index >= words.Count)
            return;

        InsultWord toRemove = words[index];
        insultBuilder.RemoveWord(toRemove);
        RefreshSlotsFromBuilder();
    }

    private void RefreshSlotsFromBuilder()
    {
        if (insultBuilder == null) return;

        var words = insultBuilder.CurrentWords;

        for (int i = 0; i < MaxSlotsUI; i++)
        {
            if (i < words.Count)
            {
                slotLabels[i].text = words[i].displayText;
            }
            else
            {
                slotLabels[i].text = "[empty]";
            }
        }
    }

    private void OnFireClicked()
    {
        if (insultBuilder == null) return;

        string insult;
        int damage;
        int gold;

        bool ok = insultBuilder.BuildInsult(out insult, out damage, out gold);
        if (!ok)
        {
            Debug.LogWarning("Cannot fire insult: no words selected.");
            return;
        }

        Debug.Log($"INSULT FIRED: \"{insult}\" | Damage = {damage}, Gold Cost = {gold}");

        // Example hook:
        // FindObjectOfType<PriestHealth>()?.ApplyDamage(damage);

        insultBuilder.ClearInsult();
        RefreshSlotsFromBuilder();
        panel.SetActive(false);
    }
}
