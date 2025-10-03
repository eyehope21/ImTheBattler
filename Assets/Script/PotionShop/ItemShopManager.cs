using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemShopManager : MonoBehaviour
{
    public ItemData[] availableItems;
    public GameObject itemButtonPrefab;
    public Transform itemGridParent;

    void Start()
    {
        PopulateShop();
    }

    void PopulateShop()
    {
        foreach (ItemData item in availableItems)
        {
            GameObject newButton = Instantiate(itemButtonPrefab, itemGridParent);
            Image iconImage = newButton.transform.Find("Icon").GetComponent<Image>();
            TMP_Text costText = newButton.transform.Find("CostText").GetComponent<TMP_Text>();

            iconImage.sprite = item.itemIcon;
            costText.text = item.itemCost.ToString() + "$";

            newButton.GetComponent<Button>().onClick.AddListener(() => PurchaseItem(item));
        }
    }

    void PurchaseItem(ItemData item)
    {
        // ✅ The purchase logic now resides in InventoryManager
        if (InventoryManager.Instance.TryPurchase(item.itemCost))
        {
            InventoryManager.Instance.AddItem(item);
        }
    }
}