using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LoginHandler : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public LoginSuccessToast loginSuccessToast; //  New public variable

    public void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ToastManager.Instance.ShowToast("Enter both email and password.");
            return;
        }

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
        .ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string errorMessage = task.Exception?.Flatten().InnerExceptions[0].Message;
                ToastManager.Instance.ShowToast("Login failed: " + errorMessage);
                return;
            }

            FirebaseUser user = task.Result.User;
            string username = user.DisplayName;

            if (!string.IsNullOrEmpty(username))
            {
                PlayerProfile.Instance.SetUsername(username);
                PlayerProfile.Instance.InitializeLevel();
            }

            //  Call the separate script for the success toast and scene change
            loginSuccessToast.ShowSuccessAndLoadScene("Login successful!");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}