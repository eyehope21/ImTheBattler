using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler : MonoBehaviour
{
    // Make sure this has a valid scene name like "Menu"
    public string fallbackScene = "Menu";

    public void GoBack()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", fallbackScene);

        //  Check if the scene name is not empty before loading
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneHistory.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("Previous scene name is empty. Loading fallback scene: " + fallbackScene);
            SceneHistory.LoadScene(fallbackScene);
        }
    }

    public void GotoRegister()
    {
        Debug.Log("Register button clicked!");
        SceneManager.LoadScene("Register");
    }

    public void GotoProfile()
    {
        Debug.Log("Profile button clicked!");
        SceneManager.LoadScene("Profile");
    }
    public void GotoLogin()
    {
        Debug.Log("Login button clicked!");
        SceneManager.LoadScene("Login");
    }

}
