using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using System.Threading.Tasks;

public class RegisterHandler : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;  // ✅ New field

    public void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // Basic validation
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ToastManager.Instance.ShowToast("Email and Password are required.");
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
            
            ToastManager.Instance.ShowToast("Registration successful!");
            SceneManager.LoadScene("ARScene"); // Or your next scene
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
