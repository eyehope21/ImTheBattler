using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStats : MonoBehaviour
{
    public string enemyName = "Enemy";
    public int maxHP = 50;
    public int attackdamage = 5;
    public float attackInterval = 10f;
    public int currentHP;

    public Slider hpSlider;
    public TMP_Text nameText; // Still needs to be a public field for BattleManager assignment
    public TMP_Text timerText;

    // --- NEW: Animator Reference ---
    private Animator animator;

    void Awake()
    {
        currentHP = maxHP;
    }

    void Start()
    {
        // Get the Animator from the Child object.
        // This is necessary because the Animator is on the child, not the root (parent) object.
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning($"Enemy '{enemyName}' is missing an Animator component in its children. Flash feedback will not work.");
        }

        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        // --- NEW: Trigger the Flash Animation ---
        if (animator != null)
        {
            // The string MUST match the Trigger parameter you set up in the Animator: "FlashTrigger"
            animator.SetTrigger("FlashTrigger");
        }
        // ----------------------------------------

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        // Only updates the HP slider. The name text is set by DungeonManager once.
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }

    private void Die()
    {
        // Placeholder for death logic:
        Debug.Log($"{enemyName} has been defeated!");

        // For now, just destroy the enemy's root object
        Destroy(gameObject);

        // **TODO:** In a real game, you would notify the BattleManager/DungeonManager here
        // (e.g., BattleManager.EnemyDefeated(this);)
    }
}