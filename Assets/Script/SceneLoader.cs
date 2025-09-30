using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // A public field to set the name of the scene to load in the Inspector
    public string nextSceneName;

    // This method is called when the button is clicked
    public void LoadDungeon()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}