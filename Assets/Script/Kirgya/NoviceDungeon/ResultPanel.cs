using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text resultText;
    public Button restartButton;
    public Button quitButton;
    public Button continueButton;

    // *** NEW FIELD: Specify the scene to load when quitting/returning to menu ***
    public string mainSceneName = "ARScene";

    private NoviceDungeonManager dungeonManager;

    private int totalCorrectAnswers = 0;
    private int totalWrongAnswers = 0;

    void Awake()
    {
        dungeonManager = FindObjectOfType<NoviceDungeonManager>();

        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(Exit); // Now calls the modified QuitGame to load the main scene
        continueButton.onClick.AddListener(ContinueGame);
    }

    // Updated method to display the score for the current round and the level-up message
    public void ShowResult(bool playerWon, int correct, int wrong, int expGained, bool didLevelUp)
    {
        panel.SetActive(true);

        // Build the result text string
        string finalResultText = "";

        if (playerWon)
        {
            totalCorrectAnswers += correct;
            totalWrongAnswers += wrong;
            finalResultText = $"You Win!\nCorrect Answers: {correct}\nWrong Answers: {wrong}\nExp +{expGained}";
            continueButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(true);
        }
        else
        {
            finalResultText = $"You Lose!\nCorrect Answers: {correct}\nWrong Answers: {wrong}";
            continueButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
        }

        // Add the level-up message to the end of the string if the player leveled up
        if (didLevelUp)
        {
            finalResultText += "\n\nCongratulations! You Leveled Up!";
        }

        resultText.text = finalResultText;
    }

    public void ShowBossVictory(int correct, int wrong, int expGained, bool didLevelUp)
    {
        panel.SetActive(true);

        totalCorrectAnswers += correct;
        totalWrongAnswers += wrong;

        string bossResultText = $"Congratulations!\n" +
                                $"You have finished the dungeon.\n \n" +
                                $"Correct Answers: {correct}\nWrong Answers: {wrong}\n" +
                                $"Exp +{expGained}\n" +
                                $"Total Correct Answers: {totalCorrectAnswers}\nTotal Wrong Answers: {totalWrongAnswers}";

        // Add the level-up message to the end of the string if the player leveled up
        if (didLevelUp)
        {
            bossResultText += "\n\nCongratulations! You Leveled Up!";
        }

        resultText.text = bossResultText;

        continueButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    private void ContinueGame()
    {
        panel.SetActive(false);
        dungeonManager.ContinueAfterVictory();
    }

    private void RestartGame()
    {
        // Reloads the current scene (Dungeon Scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // *** MODIFIED METHOD: Loads the main scene instead of quitting the application ***
    private void Exit()
    {
        if (!string.IsNullOrEmpty(mainSceneName))
        {
            // Load the main menu/hub scene
            SceneManager.LoadScene(mainSceneName);
        }
        else
        {
            Debug.LogError("Main Scene Name is not set in the Inspector! Cannot load main menu.");
        }
    }
}