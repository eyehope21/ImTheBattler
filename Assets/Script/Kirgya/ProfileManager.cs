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
        public string username = "Your Username"; // Default value
        public int level;
        public int hp;
        public int atk;
        public int def;
        public string rank = "Bronze";
        public string description = "";
    }

    // Drag these UI elements from your Hierarchy into the Inspector
    public TMP_Text levelText;
    public TMP_Text usernameText;
    public TMP_InputField descriptionInput; // Keep this assigned
    public TMP_Text hpValueText;
    public TMP_Text atkValueText;
    public TMP_Text defValueText;
    public Image rankImage;

    // Define the character limit constant
    private const int DESCRIPTION_MAX_LENGTH = 50;

    private PlayerData currentPlayer = new PlayerData(); // Initialize with defaults

    void Start()
    {
        // *** Optional: Set the character limit in code for safety ***
        if (descriptionInput != null)
        {
            descriptionInput.characterLimit = DESCRIPTION_MAX_LENGTH;
        }

        // Subscribe to the Data Manager event to refresh when stats change
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnProfileStatsChanged += LoadAndDisplayProfile;
        }

        // Initial load
        LoadAndDisplayProfile();
    }

    // This function will be called initially and by the Data Manager event.
    public void LoadAndDisplayProfile()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager not found, cannot load profile data.");
            return;
        }

        // 1. Pull the permanent stats from the Data Manager
        currentPlayer.level = GameDataManager.Instance.BaseLevel;
        currentPlayer.hp = GameDataManager.Instance.BaseMaxHP; // Use MAX HP for profile
        currentPlayer.atk = GameDataManager.Instance.BaseAttack;
        currentPlayer.def = GameDataManager.Instance.BaseDefense;

        // 2. Update the UI
        UpdateUI();
    }

    public void SaveDescription(string newDescription)
    {
        // Enforce the character limit when saving the data
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
        levelText.text = currentPlayer.level.ToString();
        usernameText.text = currentPlayer.username;

        // Note: The input field will respect the limit set in the Inspector
        descriptionInput.text = currentPlayer.description;

        // Display the BASE (MAX) HP only, as requested
        hpValueText.text = currentPlayer.hp.ToString();
        atkValueText.text = currentPlayer.atk.ToString();
        defValueText.text = currentPlayer.def.ToString();

        // PROGRAMMER: You would add logic to change the RankImage sprite here.
    }

    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed to prevent errors
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnProfileStatsChanged -= LoadAndDisplayProfile;
        }
    }
}