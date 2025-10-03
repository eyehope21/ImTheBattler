using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStats : MonoBehaviour
{
    public string enemyName = "Enemy";
    public int maxHP;
    public int attackdamage = 5;
    public float attackInterval = 10f;
    public int currentHP;

    public Slider hpSlider;
    public TMP_Text nameText;
    public TMP_Text timerText;

    private Animator animator;

    public virtual void InitializeStats()
    {
        maxHP = 50;
        enemyName = "Standard Enemy";
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
            Debug.LogWarning($"Enemy '{enemyName}' is missing an Animator component in its children. Flash feedback will not work.");
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
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyName} has been defeated!");

        Destroy(gameObject);

    }
}