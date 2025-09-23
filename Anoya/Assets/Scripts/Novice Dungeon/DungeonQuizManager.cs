using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DungeonQuizManager : MonoBehaviour
{
    public GameObject QuizPanel;
    public TMP_Text QuestionText;
    public Button[] AnswerButtons;
    public Image FeedbackFlash;
    public BattleManager battleManager;
    public QuestionData[] Questions;

    private List<QuestionData> questionPool;
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private int wrongAnswers = 0;
    private bool isQuizActive = false;

    public event Action<bool> OnAnswerEvaluated;

    private void Start()
    {
        if (QuizPanel != null)
            QuizPanel.SetActive(false);
    }

    public void StartQuiz()
    {
        isQuizActive = true;
        if (QuizPanel != null)
            QuizPanel.SetActive(true);
        if (FeedbackFlash != null)
        {
            FeedbackFlash.gameObject.SetActive(false);
        }
        correctAnswers = 0;
        wrongAnswers = 0;
        ResetQuestionPool();
        LoadQuestion();
    }

    public void EndQuiz()
    {
        isQuizActive = false;
        if (QuizPanel != null)
            QuizPanel.SetActive(false);
        StopAllCoroutines();
    }

    public int GetCorrectAnswers() => correctAnswers;
    public int GetWrongAnswers() => wrongAnswers;

    private void ResetQuestionPool()
    {
        questionPool = new List<QuestionData>(Questions);
        for (int i = 0; i < questionPool.Count; i++)
        {
            int rnd = Random.Range(i, questionPool.Count);
            (questionPool[i], questionPool[rnd]) = (questionPool[rnd], questionPool[i]);
        }
        currentQuestionIndex = 0;
    }

    private void LoadQuestion()
    {
        if (!isQuizActive)
        {
            return;
        }
        if (currentQuestionIndex >= questionPool.Count)
            ResetQuestionPool();

        QuestionData q = questionPool[currentQuestionIndex];
        QuestionText.text = q.questionText;

        List<int> indices = new List<int>();
        for (int i = 0; i < q.answers.Length; i++) indices.Add(i);
        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, indices.Count);
            (indices[i], indices[rnd]) = (indices[rnd], indices[i]);
        }

        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            if (i < q.answers.Length)
            {
                int answerIndex = indices[i];
                AnswerButtons[i].gameObject.SetActive(true);
                AnswerButtons[i].GetComponentInChildren<TMP_Text>().text = q.answers[answerIndex];
                AnswerButtons[i].onClick.RemoveAllListeners();
                QuestionData capturedQ = q;
                int capturedIndex = answerIndex;
                AnswerButtons[i].onClick.AddListener(() => OnAnswerSelected(capturedQ, capturedIndex));
            }
            else
            {
                AnswerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnAnswerSelected(QuestionData q, int chosenIndex)
    {
        bool correct = (chosenIndex == q.correctAnswerIndex);
        QuizPanel.SetActive(false);
        StartCoroutine(ShowFeedbackAndContinue(correct));
    }

    private IEnumerator ShowFeedbackAndContinue(bool isCorrect)
    {
        // Immediately invoke the event to tell DungeonManager the result
        OnAnswerEvaluated?.Invoke(isCorrect);

        // Update the scores
        if (isCorrect)
            correctAnswers++;
        else
            wrongAnswers++;

        // Only show the visual feedback on the quiz panel for INCORRECT answers
        if (!isCorrect)
        {
            if (FeedbackFlash != null)
            {
                Color flashColor = Color.red;
                flashColor.a = 0.5f;
                FeedbackFlash.color = flashColor;
                FeedbackFlash.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                FeedbackFlash.gameObject.SetActive(false);
            }
        }

        // Wait for a short duration before moving to the next question
        yield return new WaitForSeconds(1f);

        if (!isQuizActive)
        {
            yield break;
        }

        currentQuestionIndex++;
        QuizPanel.SetActive(true);
        LoadQuestion();
    }
}