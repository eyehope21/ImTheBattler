using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class LogoutHandler : MonoBehaviour
{
    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        Debug.Log("User logged out.");
        SceneManager.LoadScene("Login");
    }
}
