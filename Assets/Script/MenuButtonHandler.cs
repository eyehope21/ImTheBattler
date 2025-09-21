using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour
{


    public void LoadStory()
    {
        SaveCurrentScene();
        SceneManager.LoadScene("Story mode"); 
    }

   
    // New method to be called by the UI Butto

    public void LoadSubject()
    {
        SaveCurrentScene();
        SceneManager.LoadScene("Subjects"); 
    }


    public void LoadMenu()
    {
        SaveCurrentScene();
        SceneManager.LoadScene("Menu"); 
    }


    


    private void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("PreviousScene", currentScene);
    }
}
