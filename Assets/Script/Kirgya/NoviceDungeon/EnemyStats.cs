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
        // Only updates the HP slider. The name text is set by DungeonManager once.
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
        // Name text update logic removed to allow DungeonManager to display "Enemy 1/10: Goblin"
    }
}