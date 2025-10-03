using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossStats : MonoBehaviour
{
    public string bossName = "Boss";
    public int maxHP; 
    public int attackdamage = 20;
    public float attackInterval = 10f;
    public int currentHP;

    public Slider bossHpBar;
    public TMP_Text bossNameText;
    public TMP_Text bossTimerText;

    private Animator animator;

    public virtual void InitializeStats()
    {
        maxHP = 100;
        bossName = "Standard Boss";
    }

    void Awake()
    {
        InitializeStats();
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