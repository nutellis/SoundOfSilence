using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isFlaggedForDestruction = false;

    public Action shouldDestroy;

    void Start()
    {
        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }
    }

    void Update()
    {
        
    }

    void TakeDamage(int damage)
    {
        if (isFlaggedForDestruction == true) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            shouldDestroy?.Invoke();
        }
    }

    void Heal(int amount)
    {
        if (isFlaggedForDestruction == true) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
