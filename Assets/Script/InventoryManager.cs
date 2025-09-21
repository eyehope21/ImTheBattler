using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform inventoryGridPanel;
    public GameObject itemSlotPrefab;

   

    void Start()

    {
        // For now, we will just create 10 empty slots.
       
        PopulateInventoryGrid();

    }

    public void GoToPreviousScene()
    {

        SceneHistory.LoadScene("Menu");
    }

    void PopulateInventoryGrid()
    {
        // Clear old slots if any exist
        foreach (Transform child in inventoryGridPanel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate 10 empty slots to fill the grid
        for (int i = 0; i < 30; i++)
        {
            Instantiate(itemSlotPrefab, inventoryGridPanel);
        }
    }
}