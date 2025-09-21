using UnityEngine;
using TMPro;

public class UsernamePrompt : MonoBehaviour
{
    public GameObject usernamePanel;
    public TMP_InputField usernameInput;
    public GameObject playerInfoPanel;
    public TMP_Text playerNameText;
    public TMP_Text playerLevelText;

    void Start()
    {
        usernamePanel.SetActive(true);
        playerInfoPanel.SetActive(false);
    }

    public void OnConfirmUsername()
    {
        string enteredName = usernameInput.text.Trim();

        if (string.IsNullOrEmpty(enteredName))
        {
            ToastManager.Instance.ShowToast("Please enter a valid username.");
            return;
        }

        PlayerPrefs.SetString("PlayerName", enteredName); // Optional: Save locally
        usernamePanel.SetActive(false);
        playerInfoPanel.SetActive(true);

        playerNameText.text = $"Player: {enteredName}";
        playerLevelText.text = "Level: 1"; // Set your default or loaded level here
    }
}
