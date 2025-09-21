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
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("Previous scene name is empty. Loading fallback scene: " + fallbackScene);
            SceneManager.LoadScene(fallbackScene);
        }
    }
}