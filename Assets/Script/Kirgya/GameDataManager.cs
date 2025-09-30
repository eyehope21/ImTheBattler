
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This script holds the player's permanent, base stats.
public class GameDataManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static GameDataManager Instance { get; private set; }

    // --- Player's Base Stats (Permanent/Profile Stats) ---
    public int BaseLevel = 1;
    public int BaseMaxHP = 100;
    public int BaseAttack = 10;
    public int BaseDefense = 0; // Assuming Defense will be added later
    public int CurrentEXP = 0;
    public int ExpToNextLevel = 100;

    // Event to notify the Profile Manager when stats change
    public event Action OnProfileStatsChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // IMPORTANT: Make sure this GameObject persists across scene loads
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameDataManager initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method whenever a permanent stat (like Level or Base Attack) changes
    public void NotifyProfileUpdate()
    {
        OnProfileStatsChanged?.Invoke();
    }
}