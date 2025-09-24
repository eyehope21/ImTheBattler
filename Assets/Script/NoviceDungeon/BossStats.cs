using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        if (bossHpBar != null)
        {
            bossHpBar.maxValue = maxHP;
            bossHpBar.value = currentHP;
        }

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }
    }
}