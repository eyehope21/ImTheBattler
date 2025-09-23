using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultPanelController : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text resultText;
    public Button restartButton;
    public Button quitButton;
    public Button continueButton;

    // The levelUpText field is no longer needed since it's now part of the resultText.
    // You can remove this line and the TMP_Text component from the Inspector.
    // public TMP_Text levelUpText;

    private DungeonManager dungeonManager;

    private int totalCorrectAnswers = 0;
    private int totalWrongAnswers = 0;

    void Awake()
    {
        dungeonManager = FindObjectOfType<DungeonManager>();

        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
        continueButton.onClick.AddListener(ContinueGame);
    }

    // Updated method to display the score for the current round and the level-up message
    public void ShowResult(bool playerWon, int correct, int wrong, int expGained, bool didLevelUp)
    {
        panel.SetActive(true);
        // The levelUpText.gameObject.SetActive(didLevelUp) line is no longer needed.

        // Build the result text string
        string finalResultText = "";

        if (playerWon)
        {
            totalCorrectAnswers += correct;
            totalWrongAnswers += wrong;
            finalResultText = $"You Win!\nCorrect Answers: {correct}\nWrong Answers: {wrong}\nExp +{expGained}";
            continueButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
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
                                $"You have finished the dungeon.\n\n" +
                                $"Correct Answers: {correct}\nWrong Answers: {wrong}\n" +
                                $"Exp +{expGained}\n\n" +
                                $"Total Correct Answers: {totalCorrectAnswers}\nTotal Wrong Answers: {totalWrongAnswers}";

        // Add the level-up message to the end of the string if the player leveled up
        if (didLevelUp)
        {
            bossResultText += "\n\nCongratulations! You Leveled Up!";
        }

        resultText.text = bossResultText;

        continueButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);
    }

    private void ContinueGame()
    {
        panel.SetActive(false);
        dungeonManager.ContinueAfterVictory();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed (won't close in editor)");
    }
}