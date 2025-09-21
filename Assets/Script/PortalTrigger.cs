using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    // The name of the scene to load when the portal is entered
    public string nextSceneName = "QuizGame";

    // This is the trigger method, which detects when a GameObject enters its area
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering object is the Main Camera
        if (other.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log("Camera passed through the portal! Loading next scene...");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}