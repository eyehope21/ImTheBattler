using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class ProfileManager : MonoBehaviour
{
    // A key used to save and load the description text (Still uses PlayerPrefs for this unique text field)
    private const string DescriptionSaveKey = "PlayerProfileDescription";
    private const int DESCRIPTION_MAX_LENGTH = 50;

    // --- Stat Calculation Constants ---
    // These constants define how much stats increase per level.
    private const float BASE_HP_PER_LEVEL = 10f;
    private const float BASE_ATK_PER_LEVEL = 2f;
    private const float BASE_DEF_PER_LEVEL = 1f;

    // A local snapshot of the player's data for UI display.
    [System.Serializable]
    public class PlayerData
    {
        public string username = "Your Username"; // Assuming this is set globally somewhere else
        public int level = 1;
        public int hp = 100;
        public int atk = 10;
        public int def = 0;
        public string rank = "Bronze";
        public string description = "";

        // XP tracking for the progress bar
        public float currentXP = 0f;
        public float requiredXP = 100f;
    }

    [Header("Profile Text Fields")]
    public TMP_Text levelText;
    public TMP_Text usernameText;
    public TMP_Text hpValueText;
    public TMP_Text atkValueText;
    public TMP_Text defValueText;
    public Image rankImage;

    [Header("Description & Save Button")]
    public TMP_InputField descriptionInput;
    public Button saveDescriptionButton;

    [Header("Level Progress Circle")]
    [Tooltip("The Image component set to 'Filled' that sits behind the level number.")]
    public Image progressCircleFill;

    private PlayerData currentPlayer = new PlayerData();

    void Start()
    {
        if (descriptionInput != null)
        {
            descriptionInput.characterLimit = DESCRIPTION_MAX_LENGTH;
        }

        // Attach the saving function to the button
        if (saveDescriptionButton != null)
        {
            saveDescriptionButton.onClick.RemoveAllListeners();
            saveDescriptionButton.onClick.AddListener(SaveDescription);
        }

        // 1. Load data and display profile initially
        LoadAndDisplayProfile();

        // 2. Subscribe to the GameDataManager event to update UI whenever stats change
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnProfileStatsChanged += LoadAndDisplayProfile;
        }
    }

    void OnDestroy()
    {
        // IMPORTANT: Unsubscribe when this component is destroyed to prevent errors.
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnProfileStatsChanged -= LoadAndDisplayProfile;
        }
    }

    /// <summary>
    /// Loads all data from the GameDataManager and persistent storage.
    /// This method is called both on Start and whenever the GameDataManager notifies of a change.
    /// </summary>
    public void LoadAndDisplayProfile()
    {
        // 1. Get dynamic data from the Singleton (The Source of Truth)
        if (GameDataManager.Instance != null)
        {
            // Pull the latest core stats
            currentPlayer.level = GameDataManager.Instance.BaseLevel;
            currentPlayer.currentXP = GameDataManager.Instance.CurrentEXP;
            currentPlayer.requiredXP = GameDataManager.Instance.ExpToNextLevel;

            // Assuming username is set elsewhere, or using a fallback
            // currentPlayer.username = GameDataManager.Instance.Username; 
        }

        // 2. Load persistent data (only the description)
        currentPlayer.description = PlayerPrefs.GetString(DescriptionSaveKey, "");

        // 3. Calculate and apply stats
        CalculateStats(currentPlayer.level);

        // 4. Update all UI components
        UpdateUI();
    }

    /// <summary>
    /// Calculates HP, ATK, and DEF based on the player's current level.
    /// </summary>
    private void CalculateStats(int level)
    {
        // Use the GameDataManager's base values as the starting point (Level 1)
        int baseHP = GameDataManager.Instance != null ? GameDataManager.Instance.BaseMaxHP : 100;
        int baseATK = GameDataManager.Instance != null ? GameDataManager.Instance.BaseAttack : 10;
        int baseDEF = GameDataManager.Instance != null ? GameDataManager.Instance.BaseDefense : 0;

        // Scale the stats based on the current level
        currentPlayer.hp = baseHP + (int)((level - 1) * BASE_HP_PER_LEVEL);
        currentPlayer.atk = baseATK + (int)((level - 1) * BASE_ATK_PER_LEVEL);
        currentPlayer.def = baseDEF + (int)((level - 1) * BASE_DEF_PER_LEVEL);
    }

    /// <summary>
    /// Saves the text from the Input Field to persistent storage.
    /// </summary>
    public void SaveDescription()
    {
        if (descriptionInput == null) return;

        string newDescription = descriptionInput.text.Trim();

        // Truncate if too long
        if (newDescription.Length > DESCRIPTION_MAX_LENGTH)
        {
            currentPlayer.description = newDescription.Substring(0, DESCRIPTION_MAX_LENGTH);
        }
        else
        {
            currentPlayer.description = newDescription;
        }

        PlayerPrefs.SetString(DescriptionSaveKey, currentPlayer.description);
        PlayerPrefs.Save();

        // Update the input field text to show the saved/truncated version
        descriptionInput.text = currentPlayer.description;

        StartCoroutine(FlashSaveButton(Color.green));
    }

    /// <summary>
    /// Updates the circular progress bar fill amount based on current XP data.
    /// </summary>
    public void UpdateLevelProgress()
    {
        if (progressCircleFill == null) return;

        // Safety check
        if (currentPlayer.requiredXP <= 0)
        {
            progressCircleFill.fillAmount = 0;
            return;
        }

        float fillAmount = Mathf.Clamp01(currentPlayer.currentXP / currentPlayer.requiredXP);

        progressCircleFill.fillAmount = fillAmount;
    }

    private void UpdateUI()
    {
        // 1. Update text fields
        if (levelText != null) levelText.text = currentPlayer.level.ToString();
        if (usernameText != null) usernameText.text = currentPlayer.username;
        if (descriptionInput != null) descriptionInput.text = currentPlayer.description;
        if (hpValueText != null) hpValueText.text = currentPlayer.hp.ToString();
        if (atkValueText != null) atkValueText.text = currentPlayer.atk.ToString();
        if (defValueText != null) defValueText.text = currentPlayer.def.ToString();

        // 2. Update the progress circle
        UpdateLevelProgress();
    }

    // --- Utility/Feedback Coroutine ---
    private IEnumerator FlashSaveButton(Color color)
    {
        if (saveDescriptionButton == null) yield break;

        Color originalColor = saveDescriptionButton.image.color;
        saveDescriptionButton.image.color = color;
        yield return new WaitForSeconds(0.25f);
        saveDescriptionButton.image.color = originalColor;
    }
}