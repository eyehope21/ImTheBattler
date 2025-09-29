using UnityEngine;
using TMPro;

public class InventoryUIInitializer : MonoBehaviour
{
    public Transform gridPanel;
    public TMP_Text currencyText;

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.InitializeUI(gridPanel, currencyText);
        }
        else
        {
            Debug.LogError("InventoryManager instance is not found.");
        }
    }
}