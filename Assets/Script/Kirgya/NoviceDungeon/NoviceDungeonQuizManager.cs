using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq; // Needed for Where() and ToList()

public class NoviceDungeonQuizManager : MonoBehaviour
{
    public GameObject QuizPanel;
    public TMP_Text QuestionText;
    public Button[] AnswerButtons;
    public Image FeedbackFlash;
    public NoviceBattleManager battleManager;

    // Remove the public QuestionData[] Questions field. It's now loaded dynamically.
    // public QuestionData[] Questions; 

    // --- NEW CONFIGURATION FIELDS ---
    [Header("Quiz Filtering")]
    public Subject requiredSubject = Subject.ComputerProgramming1;
    public SchoolTerm requiredTerm = SchoolTerm.Prelim;
    public Difficulty requiredDifficulty = Difficulty.Novice;

    private QuestionData[] allLoadedQuestions; // Stores ALL questions from Resources
    private List<QuestionData> questionPool;   // Stores filtered and shuffled questions

    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private int wrongAnswers = 0;
    private bool isQuizActive = false;

    public event Action<bool> OnAnswerEvaluated;

    private void Awake()
    {
        // Load all QuestionData assets from the Resources/QuizQuestions folder
        allLoadedQuestions = Resources.LoadAll<QuestionData>("QuizQuestions");

        if (allLoadedQuestions.Length == 0)
        {
            Debug.LogError("No QuestionData ScriptableObjects found in Resources/QuizQuestions!");
        }
    }

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

        // ** NEW: Filter the questions before resetting the pool **
        FilterAndResetQuestionPool();
        LoadQuestion();
    }

    // --- NEW FILTERING METHOD ---
    private void FilterAndResetQuestionPool()
    {
        // Use LINQ to filter the questions based on the public settings
        List<QuestionData> filteredQuestions = allLoadedQuestions
            .Where(q => q.subject == requiredSubject)
            .Where(q => q.schoolTerm == requiredTerm)
            .Where(q => q.difficulty == requiredDifficulty)
            .ToList();

        if (filteredQuestions.Count == 0)
        {
            Debug.LogError($"No questions found for Subject: {requiredSubject}, Term: {requiredTerm}, Difficulty: {requiredDifficulty}!");
            // Fallback: Use all questions if none match the filter
            filteredQuestions = allLoadedQuestions.ToList();
        }

        // Shuffle the filtered list (your existing shuffle logic)
        questionPool = filteredQuestions;
        for (int i = 0; i < questionPool.Count; i++)
        {
            int rnd = Random.Range(i, questionPool.Count);
            (questionPool[i], questionPool[rnd]) = (questionPool[rnd], questionPool[i]);
        }
        currentQuestionIndex = 0;
    }

    // --- REMOVED ResetQuestionPool(), replaced by FilterAndResetQuestionPool() ---

    public void EndQuiz()
    {
        isQuizActive = false;
        if (QuizPanel != null)
            QuizPanel.SetActive(false);
        StopAllCoroutines();
    }

    public int GetCorrectAnswers() => correctAnswers;
    public int GetWrongAnswers() => wrongAnswers;

    private void LoadQuestion()
    {
        if (!isQuizActive)
        {
            return;
        }

        // If the pool is empty, we have a filtering issue (should be caught by FilterAndResetQuestionPool)
        if (questionPool == null || questionPool.Count == 0) return;

        // If we run out of questions, shuffle and restart the filtered pool
        if (currentQuestionIndex >= questionPool.Count)
            FilterAndResetQuestionPool(); // Re-shuffle the current difficulty pool

        // ... (rest of your LoadQuestion logic remains the same)
        // ...
        QuestionData q = questionPool[currentQuestionIndex];
        QuestionText.text = q.questionText;

        // ... (Answer button population and listener logic)
        // ... (The rest of LoadQuestion() is unchanged)

        // Only including the unique logic blocks for brevity
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
        OnAnswerEvaluated?.Invoke(isCorrect);
        if (isCorrect)
            correctAnswers++;
        else
            wrongAnswers++;

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