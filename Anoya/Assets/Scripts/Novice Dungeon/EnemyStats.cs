using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class EnemyStats : MonoBehaviour
{
    public event Action OnEnemyDefeated;

    public string enemyName = "Enemy";
    public int maxHP = 10;
    public int attackdamage = 10;
    public float attackInterval = 10f; // This is now a data field, not a live timer
    public int currentHP;

    public Slider hpSlider;
    public TMP_Text nameText;
    public TMP_Text timerText;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;
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

        if (nameText != null)
        {
            nameText.text = enemyName;
        }
    }
}