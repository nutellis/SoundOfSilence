using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AttackManager : MonoBehaviour
{
    public Instrument[] instruments = { };

    private Instrument activeWeapon;

    public int ultimateMeter;

    private int currentUltimate = 50;

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

            // Duck background music when instrument is summoned
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.DuckBackgroundMusic();
            }

            //this is where we animate each weapon summon.
        }
        else
        {
            Debug.LogWarning($"<color=#FFA500>No attack found for action: {action}</color>");
        }
    }

    public void DismissWeapon()
    {
        // Deactivate all weapons
        foreach (var weapon in instruments)
        {
            weapon.gameObject.SetActive(false);
        }

        activeWeapon = null;

        // Restore background music volume
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RestoreBackgroundMusic();
        }

        Debug.Log("<color=#00FF00>Weapon dismissed</color>");
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

        InsultBuilderRuntimeUI insultBuilder = GetComponentInChildren<InsultBuilderRuntimeUI>();
        if(insultBuilder != null )
        {
            insultBuilder.StartInsultBuilder();
        }
        Time.timeScale = 0.0f;


        currentUltimate = 0;
    }


    public void OnMinionDeath(int amount)
    {
        currentUltimate += amount;
        currentUltimate = Mathf.Min(currentUltimate, ultimateMeter);
        Debug.Log($"<color=#00FF00>Ultimate meter increased to {currentUltimate}/{ultimateMeter}</color>");
    }
}
