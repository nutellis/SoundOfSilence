using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [Header("Starting Gold")]
    [SerializeField] private int startingGold = 50;

    public int CurrentGold { get; private set; }

    private void Awake()
    {
        CurrentGold = startingGold;
    }

    public bool CanAfford(int amount)
    {
        return amount >= 0 && CurrentGold >= amount;
    }

    public bool Spend(int amount)
    {
        if (!CanAfford(amount))
            return false;

        CurrentGold -= amount;
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        CurrentGold += amount;
    }
}
