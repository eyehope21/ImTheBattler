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
    public GameObject levelUpPanel;

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
    public Button levelUpContinueButton;

    [Header("Scene Config")]
    public string mainSceneName = "ARScene";

    // REVERTED: Now uses a concrete manager type
    private NoviceDungeonManager dungeonManager;

    void Awake()
    {
        // REVERTED: Find the NoviceDungeonManager object to link the UI
        dungeonManager = FindObjectOfType<NoviceDungeonManager>();

        if (dungeonManager == null)
        {
            // Fallback: If Novice isn't found, try Advanced, etc. (Optional, but safe)
            // dungeonManager = FindObjectOfType<AdvancedDungeonManager>();
            // If still null, you must use a generic error message:
            Debug.LogError("DungeonResultUI failed to find an active concrete Dungeon Manager (e.g., NoviceDungeonManager).");
        }

        // --- Setup Listeners ---
        if (continueButton != null) continueButton.onClick.AddListener(ContinueGame);
        if (victoryQuitButton != null) victoryQuitButton.onClick.AddListener(OpenConfirmationPanel);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (defeatQuitButton != null) defeatQuitButton.onClick.AddListener(Exit);
        if (bossRestartButton != null) bossRestartButton.onClick.AddListener(RestartGame);
        if (bossQuitButton != null) bossQuitButton.onClick.AddListener(Exit);
        if (confirmExitButton != null) confirmExitButton.onClick.AddListener(ConfirmExitAndForfeit);
        if (cancelExitButton != null) cancelExitButton.onClick.AddListener(CloseConfirmationPanel);

        // --- Level Up Listener Setup ---
        if (levelUpContinueButton != null)
        {
            levelUpContinueButton.onClick.RemoveAllListeners();
            levelUpContinueButton.onClick.AddListener(ContinueFromLevelUp);
        }

        HideAllPanels();
    }

    // Helper to hide everything
    private void HideAllPanels()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (bossPanel != null) bossPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
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

    // --- Method to proceed from the level up panel ---
    private void ContinueFromLevelUp()
    {
        HideAllPanels();
        // This method must exist in NoviceDungeonManager
        if (dungeonManager != null) dungeonManager.ContinueFromLevelUpNotification();
    }

    // Called after a regular monster defeat
    // NOTE: Argument list has been simplified to match standard output, adjust if your manager returns more data
    public void ShowVictory(int expGained, int accumulatedExp, int playerLevel)
    {
        HideAllPanels();
        if (victoryPanel != null) victoryPanel.SetActive(true);

        string result = $"LEVEL CLEARED!\n" +
            $"EXP Gained this fight: +{expGained}\n" +
            $"\nTotal EXP Accumulated: {accumulatedExp}\n" +
            $"\nCurrent Player Level: {playerLevel}\n" +
            $"\nContinue exploring the dungeon?";

        if (victoryResultText != null) victoryResultText.text = result;
    }

    // Called after player defeat
    // NOTE: Argument list simplified
    public void ShowDefeat(int currentLevel, int runTotalCorrect, int runTotalWrong, int finalAttack, int finalHP)
    {
        HideAllPanels();
        if (defeatPanel != null) defeatPanel.SetActive(true);

        string result = $"DEFEATED on Level {currentLevel}!\n" +
            $"Correct Answers: {runTotalCorrect}\nWrong Answers: {runTotalWrong}\n" +
            $"All accumulated rewards for this run have been forfeited.";

        if (defeatResultText != null) defeatResultText.text = result;
    }

    // Called after final boss defeat
    public void ShowBossVictory(int totalExpGained, int accumulatedExp, int playerLevel)
    {
        HideAllPanels();
        if (bossPanel != null) bossPanel.SetActive(true);

        string result = $"DUNGEON CLEARED!\n" +
            $"You have finished the dungeon and earned your rewards!\n \n" +
            $"Total EXP Earned: +{totalExpGained}\n" +
            $"Final Player Level: {playerLevel}";

        if (bossResultText != null) bossResultText.text = result;
    }

    // --- Game Flow Methods ---

    private void ContinueGame()
    {
        HideAllPanels();
        // This method must exist in NoviceDungeonManager
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

    // --- Confirmation Flow Methods (Unchanged) ---

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
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    private void ConfirmExitAndForfeit()
    {
        Exit();
    }
}