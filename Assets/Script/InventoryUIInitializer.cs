using UnityEngine;
using TMPro;

public class InventoryUIInitializer : MonoBehaviour
{
    public Transform gridPanel;
    

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
           
        }
        else
        {
            Debug.LogError("InventoryManager instance is not found.");
        }
    }
}