using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ResetPasswordHandler : MonoBehaviour
{
    public TMP_InputField emailInput;

    public void ResetPassword()
    {
        string email = emailInput.text;

        if (string.IsNullOrEmpty(email))
        {
            ToastManager.Instance.ShowToast("Please enter your email.");
            return;
        }

        FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(email)
        .ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                ToastManager.Instance.ShowToast("Reset failed: " + task.Exception?.Flatten().InnerExceptions[0].Message);
                return;
            }

            ToastManager.Instance.ShowToast("Password reset email sent!, Please Check in Spam!");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
