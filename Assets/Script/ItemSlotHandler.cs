using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlotHandler : MonoBehaviour
{
    public Image itemIconImage;
    public TMP_Text itemCountText;

    //  This new method sets the item data for a slot
    public void SetItem(ItemData item)
    {
        if (item.itemIcon != null)
        {
            itemIconImage.sprite = item.itemIcon;
            itemIconImage.color = Color.white;
        }
        else
        {
            itemIconImage.color = Color.clear;
        }
        itemCountText.text = "x1"; // For now, let's assume 1 item
    }

    public void ClearSlot()
    {
        itemIconImage.sprite = null;
        itemIconImage.color = Color.clear;
        itemCountText.text = "";
    }
}