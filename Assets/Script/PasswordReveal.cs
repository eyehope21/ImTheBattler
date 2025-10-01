using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PasswordReveal : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public Image revealButtonImage;
    public Sprite hidePasswordSprite;
    public Sprite showPasswordSprite;

    private bool isPasswordHidden = true;

    public void TogglePasswordVisibility()
    {
        isPasswordHidden = !isPasswordHidden;
        if (isPasswordHidden)
        {
            // Show password as dots
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            revealButtonImage.sprite = hidePasswordSprite;
        }
        else
        {
            // Show password as text
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            revealButtonImage.sprite = showPasswordSprite;
        }

        // Re-focus the input field to apply the change
        passwordInput.ForceLabelUpdate();
    }
}