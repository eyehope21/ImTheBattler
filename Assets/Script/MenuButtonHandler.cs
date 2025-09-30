using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour
{


    public void LoadStory()
    {
        SaveCurrentScene();
        SceneHistory.LoadScene("Story mode"); 
    }

   
    // New method to be called by the UI Butto

    public void LoadSubject()
    {
        SaveCurrentScene();
        SceneHistory.LoadScene("Subjects"); 
    }


    public void LoadMenu()
    {
        SaveCurrentScene();
        SceneHistory.LoadScene("Menu"); 
    }

    private void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("PreviousScene", currentScene);
    }
}
