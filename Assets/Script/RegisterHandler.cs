using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterHandler : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;
    public TMP_InputField confirmPasswordInput;
    public RegisterSuccessToast registerSuccessToast;

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

            // After successful registration, update the user profile with the username
            Firebase.Auth.FirebaseUser newUser = task.Result.User;
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile { DisplayName = username };

            newUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(updateTask => {
                if (updateTask.IsCanceled || updateTask.IsFaulted)
                {
                    Debug.LogError("Failed to update user profile: " + updateTask.Exception);
                    return;
                }

                // All this logic should happen ONLY on a successful profile update
                Debug.Log("Registration successful! Loading Login scene...");
                PlayerProfile.Instance.SetUsername(username);
                PlayerProfile.Instance.InitializeLevel();

                // Now call the separate script for the success toast and scene change
                registerSuccessToast.ShowSuccessAndLoadScene("Registration successful!");
            });
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}