using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonResultUI : MonoBehaviour
{
    // --- Panel References (Must be assigned in Inspector) ---
    [Header("Panel GameObjects")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject bossPanel;
    public GameObject confirmationPanel;
    public GameObject levelUpPanel; // CORRECT

    // --- Victory Panel UI Elements ---
    [Header("Victory UI Elements")]
    public TMP_Text victoryResultText;
    public Button continueButton;
    public Button victoryQuitButton;

    // --- Defeat Panel UI Elements ---
    [Header("Defeat UI Elements")]
    public TMP_Text defeatResultText;
    public Button restartButton;
    public Button defeatQuitButton;

    // --- Boss Panel UI Elements ---
    [Header("Boss UI Elements")]
    public TMP_Text bossResultText;
    public Button bossRestartButton;
    public Button bossQuitButton;

    // --- Confirmation UI Elements ---
    [Header("Confirmation UI Elements")]
    public TMP_Text confirmationText;
    public Button confirmExitButton;
    public Button cancelExitButton;

    // --- NEW UI Elements for Level Up Panel ---
    [Header("Level Up UI Elements")]
    public TMP_Text levelUpText;
    public Button levelUpContinueButton; // CORRECT

    [Header("Scene Config")]
    public string mainSceneName = "ARScene";

    private NoviceDungeonManager dungeonManager;

    void Awake()
    {
        dungeonManager = FindObjectOfType<NoviceDungeonManager>();

        // --- Setup Listeners ---
        // Victory Panel
        if (continueButton != null) continueButton.onClick.AddListener(ContinueGame);
        if (victoryQuitButton != null) victoryQuitButton.onClick.AddListener(OpenConfirmationPanel);

        // Defeat Panel
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (defeatQuitButton != null) defeatQuitButton.onClick.AddListener(Exit);

        // Boss Panel
        if (bossRestartButton != null) bossRestartButton.onClick.AddListener(RestartGame);
        if (bossQuitButton != null) bossQuitButton.onClick.AddListener(Exit);

        // Confirmation Panel
        if (confirmExitButton != null) confirmExitButton.onClick.AddListener(ConfirmExitAndForfeit);
        if (cancelExitButton != null) cancelExitButton.onClick.AddListener(CloseConfirmationPanel);

        // --- CORRECTED Level Up Listener Setup ---
        if (levelUpContinueButton != null)
        {
            // Clean up existing listeners if the button was configured in the Inspector
            levelUpContinueButton.onClick.RemoveAllListeners();
            levelUpContinueButton.onClick.AddListener(ContinueFromLevelUp);
        }

        HideAllPanels();
    }

    // Helper to hide everything (Added null checks for robustness)
    private void HideAllPanels()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (bossPanel != null) bossPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(false); // Added null check for safety
    }

    // Displays Level Up panel
    public void ShowLevelUp(int newLevel)
    {
        HideAllPanels();
        if (levelUpPanel != null) levelUpPanel.SetActive(true);
        if (levelUpText != null)
        {
            levelUpText.text = $"CONGRATULATIONS!\n\nYou've reached Level {newLevel}!";
        }
    }

    // --- NEW: Method to proceed from the level up panel to the standard results ---
    private void ContinueFromLevelUp()
    {
        HideAllPanels();
        if (dungeonManager != null)
        {
            // This calls the Dungeon Manager to show the final Boss Victory screen
            dungeonManager.ContinueFromLevelUpNotification();
        }
    }

    // Called after a regular monster defeat
    public void ShowVictory(int correct, int wrong, int runTotalCorrect, int runTotalWrong)
    {
        HideAllPanels();
        if (victoryPanel != null) victoryPanel.SetActive(true);

        // Using all the accumulated data for a clearer result display
        string result = $"LEVEL CLEARED!\n" +
            $"Round Score: {correct} Correct / {wrong} Wrong\n" +
            $"\nTotal Run Score: {runTotalCorrect} Correct | {runTotalWrong} Wrong\n" +
            $"\nContinue to the next level?";

        if (victoryResultText != null) victoryResultText.text = result;
    }

    // Called after player defeat
    public void ShowDefeat(int correct, int wrong)
    {
        HideAllPanels();
        if (defeatPanel != null) defeatPanel.SetActive(true);

        string result = $"DEFEATED!\n" +
                $"Correct Answers: {correct}\nWrong Answers: {wrong}\n" +
                $"All accumulated rewards for this run have been forfeited.";

        if (defeatResultText != null) defeatResultText.text = result;
    }

    // Called after final boss defeat
    public void ShowBossVictory(int totalRunCorrect, int totalRunWrong, int totalExpGained, bool didLevelUp)
    {
        HideAllPanels();
        if (bossPanel != null) bossPanel.SetActive(true);

        string result = $"DUNGEON CLEARED!\n" +
                $"You have finished the dungeon and earned your rewards!\n \n" +
                $"Total Correct Answers: {totalRunCorrect}\nTotal Wrong Answers: {totalRunWrong}\n" +
                $"Total EXP Earned: +{totalExpGained}";

        if (bossResultText != null) bossResultText.text = result;
    }

    // --- Game Flow Methods ---

    private void ContinueGame()
    {
        HideAllPanels();
        if (dungeonManager != null) dungeonManager.ContinueAfterVictory();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        if (!string.IsNullOrEmpty(mainSceneName))
        {
            SceneManager.LoadScene(mainSceneName);
        }
        else
        {
            Debug.LogError("Main Scene Name is not set in the Inspector! Cannot load main menu.");
        }
    }

    // --- Confirmation Flow Methods ---

    private void OpenConfirmationPanel()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(true);

        if (confirmationText != null)
        {
            confirmationText.text = "Are you sure you want to exit the dungeon?\n\n" +
                        "You will forfeit all accumulated rewards for this run.";
        }
    }

    private void CloseConfirmationPanel()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(true); // Return to the victory screen
    }

    private void ConfirmExitAndForfeit()
    {
        // When the player confirms, the game exits.
        // The *forfeiture* of rewards is handled implicitly because the player is leaving
        // before the rewards were applied, or it is assumed the Exit() method handles cleanup.
        Exit();
    }
}