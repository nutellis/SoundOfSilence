using UnityEngine;
using TMPro;

public class UIButtonMerchant : MonoBehaviour
{
    [SerializeField] private TMP_Text wordLabel;
    [SerializeField] private TMP_Text costLabel;

    private InsultWord word;
    private Merchant shop;

    public void Setup(InsultWord word, Merchant shop)
    {
        wordLabel.text = word.displayText;
        costLabel.text = word.goldCost.ToString();
    }

    public void OnBuyPressed()
    {
        if (shop == null || word == null) return;

        bool success = shop.TryBuyWord(word);
        if (success)
        {
            Debug.Log("Bought: " + word.displayText);
        }
        else
        {
            Debug.Log("Could not buy: " + word.displayText);
        }
    }
}
