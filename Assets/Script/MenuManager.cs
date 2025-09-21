using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class MenuManager : MonoBehaviour
{

    public string fallbackScene = "MenuScene";
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("Login"); // your login scene name
    }

    // Example stubs for other buttons

  
    public void GoBack()
    {
        // Get the name of the previous scene from PlayerPrefs
        string previousScene = PlayerPrefs.GetString("PreviousScene", fallbackScene);

        // Load the previous scene
        SceneManager.LoadScene(previousScene);
    }




    // Placeholder methods for all the other buttons
    public void OpenMail()
    {
        Debug.Log("Mail button clicked!");
        SceneManager.LoadScene("Mail");
    }

    public void OpenQuest()
    {
        Debug.Log("Quest button clicked!");
        SceneManager.LoadScene("Quest");
    }

    public void OpenInventory()
    {
        Debug.Log("Inventory button clicked!");
        SceneManager.LoadScene("Inventory");
    }

    public void OpenLeaderboard()
    {
        Debug.Log("Leaderboard button clicked!");
        SceneManager.LoadScene("Leaderboard");
    }

    public void OpenStore()
    {
        Debug.Log("Store button clicked!");
        SceneManager.LoadScene("Store");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked!");
        SceneManager.LoadScene("Settings");
    }

    public void OpenHistoryLog()
    {
        Debug.Log("History Log button clicked!");
        SceneManager.LoadScene("HistoryLog");
    }
    public void OpenMainMenu()
    {
        Debug.Log("MainMenu button clicked!");
        SceneManager.LoadScene("ARScene");
    }
}
