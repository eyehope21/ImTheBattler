using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class RegisterHandler : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;
    public TMP_InputField confirmPasswordInput;

    public void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            ToastManager.Instance.ShowToast("All fields are required.");
            return;
        }

        if (password != confirmPassword)
        {
            ToastManager.Instance.ShowToast("Passwords do not match.");
            return;
        }

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password)
        .ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string errorMessage = task.Exception?.Flatten().InnerExceptions[0].Message;
                ToastManager.Instance.ShowToast("Registration failed: " + errorMessage);
                return;
            }

            PlayerProfile.Instance.SetUsername(username);
            PlayerProfile.Instance.InitializeLevel();

            ToastManager.Instance.ShowToast("Registration successful!");
            SceneManager.LoadScene("Login");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}