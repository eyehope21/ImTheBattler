using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Transform inventoryGridPanel;
    public GameObject itemSlotPrefab;

    public List<ItemData> inventoryItems = new List<ItemData>();
    private int playerCurrency = 100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCurrencyUI();
        PopulateInventoryGrid();
    }

    public bool TryPurchase(int cost)
    {
        if (playerCurrency >= cost)
        {
            playerCurrency -= cost;
            UpdateCurrencyUI();
            return true;
        }
        return false;
    }

    // ✅ New: A generic method to add any item type
    public void AddItem(ItemData item)
    {
        inventoryItems.Add(item);
        PopulateInventoryGrid();
    }

    void PopulateInventoryGrid()
    {
        foreach (Transform child in inventoryGridPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item in inventoryItems)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, inventoryGridPanel);
            // You will need a way to assign the item data to the new slot.
        }

        int emptySlots = 30 - inventoryItems.Count;
        for (int i = 0; i < emptySlots; i++)
        {
            Instantiate(itemSlotPrefab, inventoryGridPanel);
        }
    }

    void UpdateCurrencyUI()
    {
        // Update your currency UI here
    }
}