using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlotHandler : MonoBehaviour
{
    public Image itemIconImage;
    public TMP_Text itemCountText;

    public void SetPotion(PotionData potion)
    {
        if (potion.potionIcon != null)
        {
            itemIconImage.sprite = potion.potionIcon;
            itemIconImage.color = Color.white;
        }
        else
        {
            itemIconImage.color = Color.clear;
        }
        itemCountText.text = "x1"; // Placeholder, update if you add stackable items
    }

    public void ClearSlot()
    {
        itemIconImage.sprite = null;
        itemIconImage.color = Color.clear;
        itemCountText.text = "";
    }
}