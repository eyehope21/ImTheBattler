using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    // A class to store the player's data. (Keep this structure)
    [System.Serializable]
    public class PlayerData
    {
        public string username = "Your Username";
        public int level;
        public int hp = 100; // Example
        public int atk = 10; // Example
        public int def = 5;  // Example
        public string rank = "Bronze";
        public string description = "";
    }

    public TMP_Text levelText;
    public TMP_Text usernameText;
    public TMP_InputField descriptionInput;
    public TMP_Text hpValueText;
    public TMP_Text atkValueText;
    public TMP_Text defValueText;
    public Image rankImage;

    private const int DESCRIPTION_MAX_LENGTH = 50;

    private PlayerData currentPlayer = new PlayerData();

    void Start()
    {
        if (descriptionInput != null)
        {
            descriptionInput.characterLimit = DESCRIPTION_MAX_LENGTH;
        }

        // Pull the data from the PlayerProfile singleton
        LoadAndDisplayProfile();
    }

    public void LoadAndDisplayProfile()
    {
        if (PlayerProfile.Instance == null)
        {
            Debug.LogError("PlayerProfile not found, cannot load profile data.");
            return;
        }

        // Pull the data from the PlayerProfile singleton
        currentPlayer.username = PlayerProfile.Instance.Username;
        currentPlayer.level = PlayerProfile.Instance.Level;

        // Update the UI
        UpdateUI();
    }

    public void SaveDescription(string newDescription)
    {
        if (newDescription.Length > DESCRIPTION_MAX_LENGTH)
        {
            currentPlayer.description = newDescription.Substring(0, DESCRIPTION_MAX_LENGTH);
            Debug.LogWarning("Description was too long and has been truncated.");
        }
        else
        {
            currentPlayer.description = newDescription;
        }
    }

    private void UpdateUI()
    {
        if (levelText != null) levelText.text = currentPlayer.level.ToString();
        if (usernameText != null) usernameText.text = currentPlayer.username;
        if (descriptionInput != null) descriptionInput.text = currentPlayer.description;
        if (hpValueText != null) hpValueText.text = currentPlayer.hp.ToString();
        if (atkValueText != null) atkValueText.text = currentPlayer.atk.ToString();
        if (defValueText != null) defValueText.text = currentPlayer.def.ToString();
    }
}