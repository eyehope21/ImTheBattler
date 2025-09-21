using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    // The data structure for your questions.
    // Fill this array with your questions in the Inspector.
    public QuestionData[] questions;
    private List<QuestionData> questionPool;
    private int currentQuestionIndex = 0;

    [Header("UI References")]
    public TMP_Text questionText;
    public Button[] answerButtons; // Assign 4 buttons in inspector
    public TMP_Text feedbackText;

    void Start()
    {
        questions = new QuestionData[3];

        // Question 1
        questions[0] = new QuestionData();
        questions[0].questionText = "What is a 'bug' in programming?";
        questions[0].answers = new string[] { "A small insect", "A mistake or error", "A type of feature", "A line of code" };
        questions[0].correctAnswerIndex = 1;

        // Question 2
        questions[1] = new QuestionData();
        questions[1].questionText = "What is an 'IDE'?";
        questions[1].answers = new string[] { "Integrated Development Environment", "Internal Drive Emulator", "Interpreted Data Engine", "Interactive Display Editor" };
        questions[1].correctAnswerIndex = 0;

        // Question 3
        questions[2] = new QuestionData();
        questions[2].questionText = "Which language is used for web styling?";
        questions[2].answers = new string[] { "Java", "Python", "HTML", "CSS" };
        questions[2].correctAnswerIndex = 3;

        ResetQuestionPool();
        LoadQuestion();
    }

    void ResetQuestionPool()
    {
        questionPool = new List<QuestionData>(questions);

        // Fisher-Yates shuffle algorithm to randomize question order
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
            // If all questions have been answered, reset the quiz
            ResetQuestionPool();
        }

        QuestionData q = questionPool[currentQuestionIndex];
        questionText.text = q.questionText;

        // Shuffle answer indices to randomize button order
        List<int> indices = new List<int>();
        for (int i = 0; i < q.answers.Length; i++) indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = Random.Range(i, indices.Count);
            (indices[i], indices[rnd]) = (indices[rnd], indices[i]);
        }

        // Assign answers to buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.answers.Length)
            {
                int answerIndex = indices[i];
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = q.answers[answerIndex];

                // Clear previous listeners and add a new one
                answerButtons[i].onClick.RemoveAllListeners();
                QuestionData capturedQ = q;
                int capturedIndex = answerIndex;

                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(capturedQ, capturedIndex));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        feedbackText.text = "";
    }

    void OnAnswerSelected(QuestionData q, int chosenIndex)
    {
        if (chosenIndex == q.correctAnswerIndex)
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