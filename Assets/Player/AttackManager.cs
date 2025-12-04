using System;
using System.Linq;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public Instrument[] instruments = { };

    private Instrument activeWeapon;

    public int ultimateMeter;

    private int currentUltimate;

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
       if(action == 30)
       {
            if(currentUltimate == ultimateMeter) {
                PerformInsultAttack();
            } else
            {
                Debug.LogWarning("<color=#FFA500>Ultimate not ready!</color>");
            }
            return;
        }

        // otherwise
        activeWeapon = instruments.FirstOrDefault(attack => attack.id == action);
        if (activeWeapon != null)
        {
            Debug.Log($"<color=#00FF00>Summoned weapon: {activeWeapon.instrumentName}</color>");

            foreach (var weapon in instruments)
            {
                weapon.gameObject.SetActive(weapon.id == activeWeapon.id);
            }

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

        activeWeapon.Fire();
    }

    void PerformInsultAttack()
    {
        Debug.Log("<color=magenta>Insult Attack performed!</color>");

        //call whatever you need to call from here

        currentUltimate = 0;
    }


    public void OnMinionDeath(int amount)
    {
        currentUltimate += amount;
        currentUltimate = Mathf.Min(currentUltimate, ultimateMeter);
        Debug.Log($"<color=#00FF00>Ultimate meter increased to {currentUltimate}/{ultimateMeter}</color>");
    }
}
