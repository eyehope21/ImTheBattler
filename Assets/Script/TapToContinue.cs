using UnityEngine;
using UnityEngine.SceneManagement;

public class TapToContinue : MonoBehaviour
{
    [SerializeField] private string nextScene = "Login";

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene(nextScene);
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // Also allow mouse click for testing in Play mode
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(nextScene);
        }
#endif
    }
}
