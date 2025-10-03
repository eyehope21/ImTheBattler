using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// CHANGE: Now inherits from EnemyStatsBase
public class BossStats : MonoBehaviour
{
    public string bossName = "Boss";
    public int maxHP = 100;
    public int attackdamage = 20;
    public float attackInterval = 10f;
    public int currentHP;

    public Slider bossHpBar;
    public TMP_Text bossNameText;
    public TMP_Text bossTimerText;

    private Animator animator;

    void Awake()
    {
        currentHP = maxHP;
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning($"Boss '{bossName}' is missing an Animator component in its children. Flash feedback will not work.");
        }

        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (animator != null)
        {
            animator.SetTrigger("FlashTrigger");
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (bossHpBar != null)
        {
            bossHpBar.maxValue = maxHP;
            bossHpBar.value = currentHP;
        }
    }

    private void Die()
    {
        Debug.Log($"{bossName} has been defeated!");
        Destroy(gameObject);
    }
}