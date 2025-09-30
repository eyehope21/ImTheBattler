using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class MenuManager : MonoBehaviour
{

    public string fallbackScene = "MenuScene";
    public void GoToMenu()
    {
        SceneHistory.LoadScene("Menu");
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneHistory.LoadScene("Login"); // your login scene name
    }

    // Example stubs for other buttons

  
    public void GoBack()
    {
        // Get the name of the previous scene from PlayerPrefs
        string previousScene = PlayerPrefs.GetString("PreviousScene", fallbackScene);

        // Load the previous scene
        SceneHistory.LoadScene(previousScene);
    }




    // Placeholder methods for all the other buttons
    public void OpenMail()
    {
        Debug.Log("Mail button clicked!");
        SceneHistory.LoadScene("Mail");
    }

    public void OpenQuest()
    {
        Debug.Log("Quest button clicked!");
        SceneHistory.LoadScene("Quest");
    }

    public void OpenInventory()
    {
        Debug.Log("Inventory button clicked!");
        SceneHistory.LoadScene("Inventory");
    }

    public void OpenLeaderboard()
    {
        Debug.Log("Leaderboard button clicked!");
        SceneHistory.LoadScene("Leaderboard");
    }

    public void OpenStore()
    {
        Debug.Log("Store button clicked!");
        SceneHistory.LoadScene("Store");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked!");
        SceneHistory.LoadScene("Settings");
    }

    public void OpenHistoryLog()
    {
        Debug.Log("History Log button clicked!");
        SceneHistory.LoadScene("HistoryLog");
    }
    public void OpenMainMenu()
    {
        Debug.Log("MainMenu button clicked!");
        SceneHistory.LoadScene("ARScene");
    }
}
