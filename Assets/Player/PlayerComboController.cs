using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class PlayerComboController : MonoBehaviour
{
    int[] combo = { 1, 1, 1 };

    int comboCounter = 0;

    AttackManager attackManager;

    void Start()
    {
        InvokeRepeating(nameof(print), 0, 5);
        attackManager = GetComponent<AttackManager>();
    }

    void Update()
    {
    }

    void print()
    {
        Debug.Log($"<color=yellow>Combo is now: {combo[0] * combo[1] * combo[2]}</color>");
    }

    public void OnQ(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("<color=#00FFFF>Q</color>");
            combo[comboCounter] = 2;

            comboCounter = (comboCounter + 1) % combo.Length;
        }
    }

    public void OnW(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("<color=#00FFFF>W</color>");
            combo[comboCounter] = 3;

            comboCounter = (comboCounter + 1) % combo.Length;
        }
    }

    public void OnE(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("<color=#00FFFF>E</color>");
            combo[comboCounter] = 5;

            comboCounter = (comboCounter + 1) % combo.Length;
        }
    }

    public void OnInvoke(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("<b><color=#00FF00>Combo triggered!</color></b>");

            //get the product that give us the id of each attack
            int weaponSum = combo[0] * combo[1] * combo[2];

            attackManager.SummonWeapon(weaponSum);

        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attackManager.PerformRegularAttack();
        }
    }
}
