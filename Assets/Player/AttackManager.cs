using System.Linq;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public Instrument[] instruments = { };

    private Instrument activeWeapon;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        
    }

    public void SummonWeapon(int action)
    {
        //here we should check if the action is the ultimate attack

        // otherwise
        activeWeapon = instruments.FirstOrDefault(attack => attack.id == action);
        if (activeWeapon != null)
        {
            Debug.Log($"<color=#00FF00>Summoned weapon: {activeWeapon.instrumentName}</color>");

            //this is where we animate each weapon summon.
        }
        else
        {
            Debug.LogWarning($"<color=#FFA500>No attack found for action: {action}</color>");
        }
    }

    public void PerformRegularAttack()
    {
        if (activeWeapon == null) return;

       // activeWeapon.Fire();
    }

    void PerformInsultAttack()
    {
        Debug.Log("<color=magenta>Insult Attack performed!</color>");
    }
}
