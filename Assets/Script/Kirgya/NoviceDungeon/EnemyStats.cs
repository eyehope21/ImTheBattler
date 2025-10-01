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