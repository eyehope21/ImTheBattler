using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DungeonManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startBattlePanel;
    public GameObject quizPanel;
    public GameObject restAreaPanel;
    public GameObject victoryPanel;

    // A reference to your quiz manager script
    public QuestionManager questionManager;

    void Start()
    {
        // Start with the battle panel visible and all others hidden
        startBattlePanel.SetActive(true);
        quizPanel.SetActive(false);
        restAreaPanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    public void OnStartBattle()
    {
        startBattlePanel.SetActive(false);
        quizPanel.SetActive(true);

        // This is where you would start the quiz
        // questionManager.ResetQuestionPool();
        // questionManager.LoadQuestion();
    }

    public void OnQuizComplete()
    {
        quizPanel.SetActive(false);
        victoryPanel.SetActive(true);
    }

    public void GoToRestArea()
    {
        quizPanel.SetActive(false);
        restAreaPanel.SetActive(true);
    }

    public void OnContinue()
    {
        restAreaPanel.SetActive(false);
        victoryPanel.SetActive(false);
        startBattlePanel.SetActive(true);
    }

    public void QuitDungeon()
    {
        SceneManager.LoadScene("MenuScene"); // Replace with your Menu scene
    }
}