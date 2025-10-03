using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerStats : MonoBehaviour
{
    // --- Dependencies ---
    [Header("Dependencies")]
    public ScreenFlasher screenFlasher;
    // REMOVED: public MonoBehaviour dungeonManager;

    private bool isDefeated = false;

    // --- Core Stats (Base values will attempt to load from GameDataManager) ---
    private int baseAttack;
    private int baseMaxHP;
    private int currentHP;

    // --- Temporary Dungeon Buffs (Reset each run) ---
    private int tempAttackBuff = 0;
    private int tempHPBuff = 0;

    // UI
    public Slider hpSlider;

    // --- Leveling Variables (Will attempt to load from/Save to GameDataManager) ---
    public int CurrentLevel { get; private set; }
    private int currentEXP;
    private int expToNextLevel;

    // Public Accessors for effective stats in battle
    public int CurrentAttack { get { return baseAttack + tempAttackBuff; } }
    public int CurrentMaxHP { get { return baseMaxHP + tempHPBuff; } }

    // New event to signal a level up
    public event Action OnLevelUp;

    private void Start()
    {
        // 1. Attempt to load permanent base stats from the persistent manager, using a fallback
        LoadBaseStats();

        // 2. Initialize current HP and battle state
        currentHP = CurrentMaxHP;
        isDefeated = false;
        UpdateUI();
    }

    private void LoadBaseStats()
    {
        // Default values used if GameDataManager is NOT found
        CurrentLevel = 1;
        baseMaxHP = 100;
        baseAttack = 10;
        currentEXP = 0;
        expToNextLevel = 100;

        // --- RE-INTEGRATED GAME DATA MANAGER LOGIC ---
        // This is the logic to connect to the persistent profile manager.
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("GameDataManager not found! Using hardcoded defaults (1/100/10).");
        }
        else
        {
            // If the manager is found, load the real permanent stats
            CurrentLevel = GameDataManager.Instance.BaseLevel;
            baseMaxHP = GameDataManager.Instance.BaseMaxHP;
            baseAttack = GameDataManager.Instance.BaseAttack;
            currentEXP = GameDataManager.Instance.CurrentEXP;
            expToNextLevel = GameDataManager.Instance.ExpToNextLevel;
        }
        // --- END RE-INTEGRATED LOGIC ---

        // Reset temporary buffs for the dungeon run
        tempAttackBuff = 0;
        tempHPBuff = 0;
    }

    // Saves the permanent base stats and level back to the GameDataManager
    private void SavePermanentStats()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.BaseLevel = CurrentLevel;
            GameDataManager.Instance.BaseAttack = baseAttack;
            GameDataManager.Instance.BaseMaxHP = baseMaxHP;
            GameDataManager.Instance.CurrentEXP = currentEXP;
            GameDataManager.Instance.ExpToNextLevel = expToNextLevel;

            // Assuming GameDataManager.Instance.NotifyProfileUpdate() exists and is necessary
            // GameDataManager.Instance.NotifyProfileUpdate(); 
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        if (screenFlasher != null)
        {
            screenFlasher.FlashScreen();
        }

        UpdateUI();
        if (currentHP <= 0 && !isDefeated)
        {
            isDefeated = true;
            currentHP = 0;
            UpdateUI();

            // --- HARDCODED TO NOVICE MANAGER ---
            // Finds the *only* manager in the scene and calls the defeat method.
            NoviceDungeonManager manager = FindObjectOfType<NoviceDungeonManager>();

            if (manager != null)
            {
                manager.OnPlayerDefeated();
            }
            else
            {
                // This error will alert you if the manager is missing, which it shouldn't be.
                Debug.LogError("Player defeated, but NoviceDungeonManager not found to handle the defeat!");
            }
            // -----------------------------------
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, CurrentMaxHP);
        UpdateUI();
    }

    // Dungeon buff: ONLY affects temporary value for this dungeon run
    public void BuffAttack(int amount)
    {
        tempAttackBuff += amount;
    }

    // Dungeon buff: ONLY affects temporary value for this dungeon run
    public void BuffMaxHP(int amount)
    {
        tempHPBuff += amount;
        currentHP += amount; // Heal on buff
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = CurrentMaxHP;
            hpSlider.value = currentHP;
        }
    }

    public void GainEXP(int expAmount)
    {
        int oldLevel = CurrentLevel;

        currentEXP += expAmount;

        while (currentEXP >= expToNextLevel)
        {
            currentEXP -= expToNextLevel;
            CurrentLevel++;
            expToNextLevel = (int)(expToNextLevel * 1.5f);
            LevelUpStats();
        }

        if (CurrentLevel > oldLevel)
        {
            SavePermanentStats();
            OnLevelUp?.Invoke();
        }
        // SavePermanentStats() is called here to save the current EXP progress, even without a level up.
        SavePermanentStats();
    }

    private void LevelUpStats()
    {
        int attackIncrease = 0;
        int hpIncrease = 0;

        // Simplified level-up scaling (based on your original logic)
        if (CurrentLevel <= 10)
        {
            attackIncrease = 2;
            hpIncrease = 3;
        }
        else if (CurrentLevel <= 20)
        {
            attackIncrease = 3;
            hpIncrease = 5;
        }
        else if (CurrentLevel <= 30)
        {
            attackIncrease = 4;
            hpIncrease = 8;
        }
        else // Fallback for higher levels
        {
            attackIncrease = 5;
            hpIncrease = 9;
        }

        baseAttack += attackIncrease;
        baseMaxHP += hpIncrease;
        currentHP += hpIncrease;

        Debug.Log($"Leveled up to Level {CurrentLevel}! Base Attack: {baseAttack}, Base HP: {baseMaxHP}.");
        UpdateUI();
    }
}