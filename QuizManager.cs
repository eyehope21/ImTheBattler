using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public QuestionData[] questions;       // All questions
    private List<QuestionData> questionPool;
    private int currentQuestionIndex = 0;

    [Header("UI References")]
    public TMP_Text questionText;
    public Button[] answerButtons; // Assign 4 buttons in inspector
    public TMP_Text feedbackText;
  
    void Start()
    {
        ResetQuestionPool();   // initialize and shuffle
        LoadQuestion();        // then show first
    }

    void ResetQuestionPool()
    {
        questionPool = new List<QuestionData>(questions);

        // Fisher-Yates shuffle
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

        // Shuffle answer indices
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

                answerButtons[i].onClick.RemoveAllListeners();

                // Capture BOTH the chosen index and the current question
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
