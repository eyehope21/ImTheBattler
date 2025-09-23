using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdentificationManager : MonoBehaviour
{
    public QuestionData[] questions;          // All identification questions
    private List<QuestionData> questionPool;
    private int currentQuestionIndex = 0;

    [Header("UI References")]
    public TMP_Text questionText;             // To display the question
    public TMP_InputField answerInputField;   // Player types answer here
    public Button submitButton;               // Submit button for answer
    public TMP_Text feedbackText;             // Correct/Wrong feedback

    void Start()
    {
        ResetQuestionPool();
        LoadQuestion();

        // Assign submit button listener
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmitAnswer);
    }

    void ResetQuestionPool()
    {
        questionPool = new List<QuestionData>(questions);

        // Shuffle questions using Fisher-Yates
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
        answerInputField.text = "";      // Clear previous input
        feedbackText.text = "";          // Clear previous feedback
        answerInputField.ActivateInputField(); // Focus input field
    }

    void OnSubmitAnswer()
    {
        QuestionData q = questionPool[currentQuestionIndex];
        string playerAnswer = answerInputField.text.Trim();

        if (string.Equals(playerAnswer, q.correctAnswerText, System.StringComparison.OrdinalIgnoreCase))
        {
            feedbackText.text = "Correct!";
        }
        else
        {
            feedbackText.text = "Wrong!";
        }

        currentQuestionIndex++;
        Invoke("LoadQuestion", 1.2f); // Load next question after short delay
    }
}