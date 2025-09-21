using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthGuard : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn", 0) != 1)
        {
            Debug.Log("User not logged in. Redirecting to LoginScene.");
            SceneManager.LoadScene("LoginScene");
        }
    }
}
