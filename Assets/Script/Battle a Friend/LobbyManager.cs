using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text lobbyCodeText;
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
        lobbyCodeText.text = $"Lobby Code: {hostCode}";
    }

    void UpdatePlayerSlots()
    {
        // In a real project, this would be updated by a network event
        if (PlayerProfile.Instance != null && playerSlots.Count > 0)
        {
            // Display the host's username
            TMP_Text hostName = playerSlots[0].GetComponentInChildren<TMP_Text>();
            if (hostName != null)
            {
                hostName.text = PlayerProfile.Instance.Username;
            }
        }

        // Player 2 slot is empty until a player joins
        if (playerSlots.Count > 1)
        {
            playerSlots[1].SetActive(false);
        }
    }

    // Call this when the "Start Game" button is clicked
    public void StartGame()
    {
        Debug.Log("Starting the game...");
        // Add game start logic here
    }
}