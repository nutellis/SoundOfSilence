using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isFlaggedForDestruction = false;

    public Action shouldDestroy;
    public Animator animator;

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

    public void TakeDamage(int damage)
    {
        if (isFlaggedForDestruction == true) return;

     //   animator.SetTrigger("Hit");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            shouldDestroy?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (isFlaggedForDestruction == true) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
