using System;
using UnityEngine;
using UnityEngine.UI;

public class MinionHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isFlaggedForDestruction = false;

    public Action shouldDestroy;
    public Animator animator;

    [Header("Health Bar UI")]
    public Image healthFillImage;


    void Start()
    {
        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
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
        UpdateHealthUI();
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
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (currentHealth + 0.0f) / maxHealth;
        }
    }
}
