using System.Linq;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public PlayerAttack[] playerAttacks = { };

    private PlayerAttack activeAttack;

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
        activeAttack = playerAttacks.FirstOrDefault(attack => attack.id == action);
        if (activeAttack != null)
        {
            Debug.Log($"<color=#00FF00>Summoned weapon: {activeAttack.attackName}</color>");

            //this is where we animate each weapon summon.
        }
        else
        {
            Debug.LogWarning($"<color=#FFA500>No attack found for action: {action}</color>");
        }
    }

    public void PerformRegularAttack()
    {
        if (activeAttack == null) return;

        if (Time.time > activeAttack.lastAttackTime)
        {
            Debug.Log($"<color=#00FFFF>{activeAttack.attackName} performing!</color>");

            //trigger animation and logic

            //where the logic for each attack comes? that is a good idea.

            //trigger cooldown on the attack
            activeAttack.lastAttackTime = Time.time + activeAttack.attackCooldown;
        }
        else
        {
            Debug.LogWarning($"<color=yellow>{activeAttack.attackName} is on cooldown</color>");
        }
    }

    void PerformInsultAttack()
    {
        Debug.Log("<color=magenta>Insult Attack performed!</color>");
    }
}
