using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneHistory : MonoBehaviour
{
    private static Stack<string> history = new Stack<string>();

    private void Awake()
    {
        if (FindObjectsOfType<SceneHistory>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public static void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        history.Push(currentScene);
        SceneManager.LoadScene(sceneName);
    }

    public static void Back()
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