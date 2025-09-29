using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotionShopManager : MonoBehaviour
{
    public PotionData[] availablePotions;
    public GameObject potionButtonPrefab; // ✅ This prefab needs Icon and CostText children
    public Transform potionGridParent;
    public TMP_Text shopMessageText;
    public TMP_Text shopCurrencyText; // ✅ New: For displaying player's currency in the shop

    void Start()
    {
        PopulateShop();
        UpdateShopCurrencyUI(); // Display currency when the shop opens
    }

    void PopulateShop()
    {
        // Clear old buttons if any exist
        foreach (Transform child in potionGridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (PotionData potion in availablePotions)
        {
            GameObject newButton = Instantiate(potionButtonPrefab, potionGridParent);

            Image iconImage = newButton.transform.Find("Icon").GetComponent<Image>();
            TMP_Text costText = newButton.transform.Find("CostText").GetComponent<TMP_Text>();

            iconImage.sprite = potion.potionIcon;
            costText.text = potion.potionCost.ToString() + "Coins"; // Display potion cost

            newButton.GetComponent<Button>().onClick.AddListener(() => PurchasePotion(potion)); // Attach buy logic
        }
    }

    void PurchasePotion(PotionData potion)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager instance is not found. Cannot purchase potion.");
            shopMessageText.text = "Shop Error!";
            return;
        }

        if (InventoryManager.Instance.TryPurchase(potion.potionCost)) // Check if player has enough coins
        {
            InventoryManager.Instance.AddPotion(potion); // Add item to inventory
            shopMessageText.text = "Purchased " + potion.potionName + "!";
            UpdateShopCurrencyUI(); // Update currency in shop UI
        }
        else
        {
            shopMessageText.text = "Not enough money for " + potion.potionName + "!"; // Insufficient coins message
        }
    }

    // New: Update shop's currency display
    void UpdateShopCurrencyUI()
    {
        if (InventoryManager.Instance != null && shopCurrencyText != null)
        {
            shopCurrencyText.text = "Your Gold: Coins" + InventoryManager.Instance.PlayerCurrency.ToString();
        }
        else
        {
            Debug.LogWarning("Shop currency UI Text not assigned or InventoryManager not found.");
        }
    }
}