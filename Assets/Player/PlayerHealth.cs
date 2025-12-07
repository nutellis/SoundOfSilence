using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isFlaggedForDestruction = false;

    public Action shouldDestroy;
    public Animator animator;

    public GameObject playerDeathUI;


    void Start()
    {
        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
        }
        if(playerDeathUI != null)
        {
            playerDeathUI.SetActive(false);
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
            playerDeathUI.SetActive(true);
            Time.timeScale = 0f;
            
            //disable all action components like camera 
            PlayerInput playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
                Cursor.lockState = CursorLockMode.Confined;
            }

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
    //    if (healthFillImage != null)
    //    {
    //        healthFillImage.fillAmount = (currentHealth + 0.0f) / maxHealth;
    //    }
    }
}
