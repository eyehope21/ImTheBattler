using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    // A class to store the player's data.
    [System.Serializable]
    public class PlayerData
    {
        public string username;
        public int level;
        public int hp;
        public int atk;
        public int def;
        public string rank;
        public string description;
    }

    // Drag these UI elements from your Hierarchy into the Inspector
    public TMP_Text levelText;
    public TMP_Text usernameText;
    public TMP_InputField descriptionInput;
    public TMP_Text hpValueText;
    public TMP_Text atkValueText;
    public TMP_Text defValueText;
    public Image rankImage;
    public Image avatarImage;

    private PlayerData currentPlayer;

    // This function will be called by your programmer to get the latest data.
    public void LoadAndDisplayProfile(PlayerData playerData)
    {
        currentPlayer = playerData;
        UpdateUI();
    }

    // For your programmer: This function loads the player's data.
    private void LoadProfileData()
    {
        // PROGRAMMER: This is where you would get the real player data.
        // For example: currentPlayer = dataManager.GetPlayerProfile();

        // For testing, we'll use example data.
        currentPlayer = new PlayerData
        {
            username = "Your Username",
            level = 1,
            hp = 10,
            atk = 10,
            def = 0,
            rank = "Bronze",
            description = ""
        };

        UpdateUI();
    }

    // This is for your description box. It saves the input text.
    public void SaveDescription(string newDescription)
    {
        currentPlayer.description = newDescription;
    }

    // This function updates the UI with the current data.
    // The programmer should call this after a level-up or stat change.
    private void UpdateUI()
    {
        levelText.text = currentPlayer.level.ToString();
        usernameText.text = currentPlayer.username;
        descriptionInput.text = currentPlayer.description;

        hpValueText.text = currentPlayer.hp.ToString();
        atkValueText.text = currentPlayer.atk.ToString();
        defValueText.text = currentPlayer.def.ToString();

        // PROGRAMMER: You would add logic to change the RankImage sprite here
        // based on the player's rank value.
        // Example: rankImage.sprite = Resources.Load<Sprite>("Ranks/" + currentPlayer.rank);
    }

    void Start()
    {
        LoadProfileData();
    }
}