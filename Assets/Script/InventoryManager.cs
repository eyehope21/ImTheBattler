using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject itemSlotPrefab; // Assign your ItemSlot prefab here

    // UI variables for the Inventory Scene, set by InventoryUIInitializer
    private Transform inventoryGridPanel;
    private TMP_Text currencyText;

    public List<PotionData> inventoryItems = new List<PotionData>();
    public int PlayerCurrency { get; private set; } // Make currency publicly accessible but set internally

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerCurrency(); // Load currency when the manager awakens
        }
        else
        {
            Debug.LogWarning("Duplicate InventoryManager detected, destroying new one.");
            Destroy(gameObject);
        }
    }

    // Call this when the Inventory scene loads to connect its specific UI elements
    public void InitializeUI(Transform grid, TMP_Text currency)
    {
        inventoryGridPanel = grid;
        currencyText = currency;
        UpdateCurrencyUI();
        PopulateInventoryGrid();
    }

    public bool TryPurchase(int cost)
    {
        if (PlayerCurrency >= cost)
        {
            PlayerCurrency -= cost;
            SavePlayerCurrency(); // Save currency after spending
            UpdateCurrencyUI();
            return true;
        }
        return false;
    }

    public void AddPotion(PotionData potion)
    {
        inventoryItems.Add(potion);
        PopulateInventoryGrid();
    }

    void PopulateInventoryGrid()
    {
        if (inventoryGridPanel == null || itemSlotPrefab == null)
        {
            Debug.LogWarning("Inventory UI not initialized or itemSlotPrefab is missing. Cannot populate grid.");
            return;
        }

        foreach (Transform child in inventoryGridPanel)
        {
            Destroy(child.gameObject);
        }

        // Display actual items
        foreach (PotionData potion in inventoryItems)
        {
            GameObject newSlotGO = Instantiate(itemSlotPrefab, inventoryGridPanel);
            ItemSlotHandler slotHandler = newSlotGO.GetComponent<ItemSlotHandler>();
            if (slotHandler != null)
            {
                slotHandler.SetPotion(potion);
            }
            else
            {
                Debug.LogError("ItemSlotHandler missing on itemSlotPrefab.");
            }
        }

        // Fill remaining slots with empty ones (up to 30 slots)
        int emptySlots = 30 - inventoryItems.Count;
        for (int i = 0; i < emptySlots; i++)
        {
            GameObject emptySlotGO = Instantiate(itemSlotPrefab, inventoryGridPanel);
            ItemSlotHandler slotHandler = emptySlotGO.GetComponent<ItemSlotHandler>();
            if (slotHandler != null)
            {
                slotHandler.ClearSlot();
            }
        }
    }

    void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = "Coins" + PlayerCurrency.ToString();
        }
        else
        {
            Debug.LogWarning("Currency UI Text not assigned in InventoryManager.");
        }
    }

    // ✅ New: Load currency from PlayerPrefs
    void LoadPlayerCurrency()
    {
        PlayerCurrency = PlayerPrefs.GetInt("PlayerCurrency", 500); // Default to 100 if not found
    }

    // ✅ New: Save currency to PlayerPrefs
    void SavePlayerCurrency()
    {
        PlayerPrefs.SetInt("PlayerCurrency", PlayerCurrency);
        PlayerPrefs.Save(); // Save changes immediately
    }
}