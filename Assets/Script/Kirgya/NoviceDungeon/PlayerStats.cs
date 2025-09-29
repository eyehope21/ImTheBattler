using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerStats : MonoBehaviour
{
    private bool isDefeated = false;

    // Change these to public fields to be managed by the new logic
    public int baseAttack = 10;
    public int maxHP = 100;
    public int currentHP;

    // UI
    public Slider hpSlider;

    // --- New variables for leveling ---
    private int currentLevel = 1; // Made private for encapsulation
    public int CurrentLevel { get { return currentLevel; } }
    public int currentEXP = 0;
    public int expToNextLevel = 100;

    // New event to signal a level up
    public event Action OnLevelUp;

    private void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
        UpdateUI();
        if (currentHP <= 0 && !isDefeated)
        {
            isDefeated = true;
            currentHP = 0;
            UpdateUI();
            FindObjectOfType<NoviceDungeonManager>().OnPlayerDefeated();
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        UpdateUI();
    }

    public void BuffAttack(int amount)
    {
        baseAttack += amount;
    }

    public void BuffMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount; // Restore HP on level up
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

    public void GainEXP(int expAmount)
    {
        int oldLevel = currentLevel; // Store the level before gaining EXP

        currentEXP += expAmount;
        while (currentEXP >= expToNextLevel) // Use a while loop to handle multiple level-ups
        {
            currentEXP -= expToNextLevel;
            currentLevel++;
            expToNextLevel = (int)(expToNextLevel * 1.5f); // Increase requirement for next level
            LevelUpStats(); // Call a separate method for stat buffs
        }

        if (currentLevel > oldLevel) // Check if the level has actually increased
        {
            // A level-up occurred, fire the event
            if (OnLevelUp != null)
            {
                OnLevelUp();
            }
        }
    }

    private void LevelUpStats()
    {
        // Permanently increase stats based on level
        if (currentLevel <= 10)
        {
            BuffAttack(2);
            BuffMaxHP(3);
        }
        else if (currentLevel <= 20)
        {
            BuffAttack(3);
            BuffMaxHP(5);
        }
        else if (currentLevel <= 30)
        {
            BuffAttack(4);
            BuffMaxHP(8);
        }
        else if (currentLevel <= 40)
        {
            BuffAttack(5);
            BuffMaxHP(9);
        }
        else if (currentLevel <= 50)
        {
            BuffAttack(6);
            BuffMaxHP(10);
        }
        else
        {
            BuffAttack(0);
            BuffMaxHP(0);
        }
        Debug.Log($"Leveled up to Level {currentLevel}! Attack is now {baseAttack}, HP is now {maxHP}.");
        UpdateUI();
    }
}