using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneHistory : MonoBehaviour
{
    private static Stack<string> history = new Stack<string>();

    private void Awake()
    {
        // Ensures only one instance of this script exists and it persists between scenes.
        if (FindObjectsOfType<SceneHistory>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public static void LoadScene(string sceneName)
    {
        // Save the current scene before loading a new one.
        string currentScene = SceneManager.GetActiveScene().name;
        history.Push(currentScene);
        SceneManager.LoadScene(sceneName);
    }

    public static void GoBack()
    {
        if (history.Count > 0)
        {
            string lastScene = history.Pop();
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogWarning("No previous scene in history!");
        }
    }
}