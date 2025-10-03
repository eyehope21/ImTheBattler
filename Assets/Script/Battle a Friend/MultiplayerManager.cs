using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
    // Reference to the input field for the join code
    public TMP_InputField joinCodeInput;
    public List<GameObject> playerSlots = new List<GameObject>(); // Use a list for player slots

    // For this example, we'll use a simple random number
    private string hostCode;

    void Start()
    {
        GenerateHostCode();
        UpdatePlayerSlots();
    }

    void GenerateHostCode()
    {
        hostCode = Random.Range(1000, 9999).ToString();
        // This would be displayed in the Host Lobby scene
        Debug.Log($"Host Code: {hostCode}");
    }

    void UpdatePlayerSlots()
    {
        // In a real project, this would be updated by a network event
        // For now, we'll just show the host
        playerSlots[0].SetActive(true);
        playerSlots[1].SetActive(false);
    }

    // Call this when the "Host a Battle" button is clicked
    public void HostBattle()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Hosting a new battle...");
        SceneManager.LoadScene("SearchAFriend");
    }

    // Call this when the "Join a Battle" button is clicked
    public void JoinBattle(string hostCode)
    {
        Debug.Log($"Attempting to join lobby with code: {hostCode}");
        SceneManager.LoadScene("SearchAFriend");
    }
}