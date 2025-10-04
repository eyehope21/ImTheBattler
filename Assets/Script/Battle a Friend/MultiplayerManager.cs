using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MultiplayerManager : MonoBehaviour
{
    // Reference to the pop-up panel for joining a lobby
    public GameObject joinLobbyPopup;
    public TMP_InputField joinCodeInput;

    // Call this when the "Battle a Friend" button is clicked in the ARScene
    public void GoToRoomScene()
    {
        SceneManager.LoadScene("Room");
    }

    // Call this when the "Search a Friend" button is clicked in the Room scene
    public void HostBattle()
    {
        Debug.Log("Hosting a new battle...");
        SceneManager.LoadScene("SearchAFriend");
    }

    // Call this when the "Join a Friend" button is clicked
    public void ShowJoinPopup()
    {
        joinLobbyPopup.SetActive(true);
    }

    // Called when player confirms the code in the pop-up
    public void JoinBattle(string hostCode)
    {
        Debug.Log($"Attempting to join lobby with code: {hostCode}");
        SceneManager.LoadScene("TheJoinAFriend");
    }
}