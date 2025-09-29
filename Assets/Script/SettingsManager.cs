using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class SettingsManager : MonoBehaviour
{
    
    public void ToggleSound()
    {
        // This is a placeholder. You would implement your sound toggling logic here.
        Debug.Log("Sound Toggled!");
        // Example: if (AudioListener.volume == 1) { AudioListener.volume = 0; } else { AudioListener.volume = 1; }
    }

    // Call this method when the SignOut button is clicked
    public void SignOut()
    {
        // Sign out from Firebase
        FirebaseAuth.DefaultInstance.SignOut();
        // Clear the login flag
        PlayerPrefs.SetInt("IsLoggedIn", 0);
        // Load the Login scene
        SceneManager.LoadScene("Login");
    }

    // Call this method when the Quit button is clicked
    public void QuitGame()
    {
        // This will quit the application.
        Application.Quit();
    }

    // Call this method when the Credits button is clicked
    public void ShowCredits()
    {
        // This is a placeholder. You would implement your credits logic here.
        Debug.Log("Showing Credits!");
        // Example: SceneManager.LoadScene("CreditsScene");
    }

    // Call this method when the Back button is clicked
    public void GoBack()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "Menu");
        SceneManager.LoadScene(previousScene);
    }
}