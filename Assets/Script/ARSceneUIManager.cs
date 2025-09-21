using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneUIManager : MonoBehaviour
{
    public void OnStoryModePressed()
    {
        Debug.Log("Story Mode clicked!");
        // Load story scene if needed
    }

    public void OnBattleFriendPressed()
    {
        Debug.Log("Battle a Friend clicked!");
        // Load battle friend scene or trigger matchmaking
    }

    public void OnMenuPressed()
    {
        SceneManager.LoadScene("Menu"); // Make sure "Menu" is added to Build Settings
    }
}
