using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToFQuestionManager : MonoBehaviour
{
    public QuestionData[] questions;         // All True/False questions
    private List<QuestionData> questionPool;
    private int currentQuestionIndex = 0;

    [Header("UI References")]
    public TMP_Text questionText;
    public Button trueButton;
    public Button falseButton;
    public TMP_Text feedbackText;

    void Start()
    {
        ResetQuestionPool();
        LoadQuestion();

        trueButton.onClick.RemoveAllListeners();
        falseButton.onClick.RemoveAllListeners();

        trueButton.onClick.AddListener(() => OnAnswerSelected(true));
        falseButton.onClick.AddListener(() => OnAnswerSelected(false));
    }

    void ResetQuestionPool()
    {
        questionPool = new List<QuestionData>(questions);

        // Shuffle questions
        for (int i = 0; i < questionPool.Count; i++)
        {
            int rnd = Random.Range(i, questionPool.Count);
            (questionPool[i], questionPool[rnd]) = (questionPool[rnd], questionPool[i]);
        }

        currentQuestionIndex = 0;
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex >= questionPool.Count)
        {
            ResetQuestionPool();
        }

        QuestionData q = questionPool[currentQuestionIndex];

        questionText.text = q.questionText;
        feedbackText.text = "";
    }

    void OnAnswerSelected(bool playerChoice)
    {
        QuestionData q = questionPool[currentQuestionIndex];

        if (playerChoice == q.correctAnswerBool)
        {
            feedbackText.text = "Correct!";
        }
        else
        {
            feedbackText.text = "Wrong!";
        }

        currentQuestionIndex++;
        Invoke("LoadQuestion", 1.2f);
    }
}