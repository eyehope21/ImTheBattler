using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionShopManager : MonoBehaviour
{
    public PotionData[] availablePotions;
    public GameObject potionButtonPrefab;
    public Transform potionGridParent;

    void Start()
    {
        PopulateShop();
    }

    void PopulateShop()
    {
        foreach (PotionData potion in availablePotions)
        {
            GameObject newButton = Instantiate(potionButtonPrefab, potionGridParent);
            // Get the image and text components
            Image iconImage = newButton.transform.Find("Icon").GetComponent<Image>();
            TMP_Text costText = newButton.transform.Find("CostText").GetComponent<TMP_Text>();

            // Assign the potion data to the button
            iconImage.sprite = potion.potionIcon;
            costText.text = potion.potionCost.ToString() + "$";

            // Add a listener to handle the purchase
            newButton.GetComponent<Button>().onClick.AddListener(() => PurchasePotion(potion));
        }
    }

    void PurchasePotion(PotionData potion)
    {
        Debug.Log("Purchased " + potion.potionName + " for " + potion.potionCost + "$.");
        // Add actual purchase logic here
    }
}