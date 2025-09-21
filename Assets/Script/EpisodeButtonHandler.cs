using UnityEngine;
using UnityEngine.SceneManagement;

public class EpisodeButtonHandler : MonoBehaviour
{
    // Public variable to set the name of the scene to load in the Unity Inspector
    public string StoryContentScene;

    // This method will be called when the Episode 1 button is clicked
    public void LoadCutsceneScene()
    {
        // Check if the scene name is not empty
        if (!string.IsNullOrEmpty(StoryContentScene))
        {
            // Load the specified scene
            SceneManager.LoadScene(StoryContentScene);
        }
        else
        {
            Debug.LogError("Cutscene scene name is not set in the inspector!");
        }
    }
}