using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NoviceDungeonQuizManager : MonoBehaviour
{
    public GameObject QuizPanel;
    public TMP_Text QuestionText;
    public Button[] AnswerButtons;
    public Image FeedbackFlash;
    public NoviceBattleManager battleManager;
    private Subject selectedSubject;
    private SchoolTerm selectedTerm;
    private Difficulty selectedDifficulty;
    private List<QuestionData> masterQuestionPool;
    private List<QuestionData> activeQuestionPool;
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private int wrongAnswers = 0;
    private bool isQuizActive = false;
    public event Action<bool> OnAnswerEvaluated;

    private void Awake()
    {
        // Load all QuestionData assets from any Resources folder
        masterQuestionPool = Resources.LoadAll<QuestionData>("").ToList();
        if (masterQuestionPool.Count == 0)
        {
            Debug.LogError("No QuestionData ScriptableObjects found in Resources folders! Quiz will not function.");
        }
    }

    private void Start()
    {
        if (QuizPanel != null)
            QuizPanel.SetActive(false);
    }

    public void SetDungeonFilters(Subject subject, SchoolTerm term, Difficulty difficulty)
    {
        selectedSubject = subject;
        selectedTerm = term;
        selectedDifficulty = difficulty;
        Debug.Log($"Dungeon Filters Received: Subject={selectedSubject}, Term={selectedTerm}, Difficulty={selectedDifficulty}");
        PopulateActiveQuestionPool();
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
        currentQuestionIndex = 0;
        ShuffleActivePool();
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

    private void ShuffleActivePool()
    {
        if (activeQuestionPool == null) return;
        // Fisher-Yates shuffle
        for (int i = 0; i < activeQuestionPool.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, activeQuestionPool.Count);
            (activeQuestionPool[i], activeQuestionPool[rnd]) = (activeQuestionPool[rnd], activeQuestionPool[i]);
        }
    }

    private void PopulateActiveQuestionPool()
    {
        if (masterQuestionPool == null || masterQuestionPool.Count == 0)
        {
            activeQuestionPool = new List<QuestionData>();
            Debug.LogError("Master Question Pool is empty! Are QuestionData assets in a Resources folder?");
            return;
        }

        // 1. Attempt to filter by all three criteria (Difficulty, Subject, Term)
        activeQuestionPool = masterQuestionPool
          .Where(q =>
            q.difficulty == selectedDifficulty &&
            q.subject == selectedSubject &&
            q.schoolTerm == selectedTerm
          )
          .ToList();

        if (activeQuestionPool.Count == 0)
        {
            Debug.LogWarning($"No questions found for the strict filters: {selectedSubject}, {selectedTerm}, {selectedDifficulty}. Attempting fallback filter.");

            // --- FALLBACK FIX: Try filtering by only Subject and Term ---
            activeQuestionPool = masterQuestionPool
                .Where(q =>
                    q.subject == selectedSubject &&
                    q.schoolTerm == selectedTerm
                )
                .ToList();

            if (activeQuestionPool.Count == 0)
            {
                Debug.LogError($"CRITICAL: Still no questions found after fallback. Check your QuestionData properties.");
            }
            else
            {
                Debug.LogWarning($"Fallback activated: Loaded {activeQuestionPool.Count} questions by Subject and Term only (Difficulty filter ignored).");
            }
        }

        ShuffleActivePool();
    }

    private void LoadQuestion()
    {
        // Check 1: Quiz state and pool status
        if (!isQuizActive)
        {
            Debug.LogWarning("LoadQuestion called but quiz is inactive. Aborting.");
            return;
        }
        if (activeQuestionPool == null || activeQuestionPool.Count == 0)
        {
            Debug.LogError("Quiz pool is empty! Cannot load question.");
            EndQuiz();
            return;
        }

        // --- FIX: ITERATIVE SEARCH FOR A VALID QUESTION ---
        QuestionData q = null;
        int initialQuestionCount = activeQuestionPool.Count * 2; // Allow checking twice the pool size, just in case

        for (int i = 0; i < initialQuestionCount; i++)
        {
            // Handle wrapping the index or reshuffling if we hit the end
            if (currentQuestionIndex >= activeQuestionPool.Count)
            {
                ShuffleActivePool();
                currentQuestionIndex = 0;
                if (activeQuestionPool.Count == 0) break; // If pool somehow became empty after shuffle
            }

            QuestionData currentQ = activeQuestionPool[currentQuestionIndex];

            // Validation Check (The one generating your error)
            // Use Trim() to catch questions that contain only whitespace characters
            bool textInvalid = string.IsNullOrEmpty(currentQ.questionText) || string.IsNullOrWhiteSpace(currentQ.questionText);
            bool answersInvalid = currentQ.answers == null || currentQ.answers.Length == 0;

            if (textInvalid || answersInvalid)
            {
                // --- DETAILED LOGGING FIX ---
                string answersLength = (currentQ.answers == null) ? "NULL" : currentQ.answers.Length.ToString();
                Debug.LogError($"Question '{currentQ.name}' failed validation! " +
                               $"Is Text Empty: {textInvalid}. " +
                               $"Answers Array Length: {answersLength}. Skipping.");
                // ----------------------------

                currentQuestionIndex++; // Move to the next question
                continue; // Skip this bad question and check the next one in the loop
            }

            // Valid question found! Assign it and break the search loop.
            q = currentQ;
            break;
        }
        // --- END FIX ---


        // Check 2: Did the search fail?
        if (q == null)
        {
            Debug.LogError("Exhausted all questions in the pool. None were valid. Ending quiz.");
            EndQuiz();
            return;
        }

        // --- UI SETUP START ---

        QuestionText.text = q.questionText;

        // Ensure the question text object is active
        if (QuestionText.gameObject.activeInHierarchy == false)
        {
            QuestionText.gameObject.SetActive(true);
        }

        // Prepare indices for answer shuffling
        List<int> indices = new List<int>();
        for (int i = 0; i < q.answers.Length; i++) indices.Add(i);

        // Shuffle indices
        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, indices.Count);
            (indices[i], indices[rnd]) = (indices[rnd], indices[i]);
        }

        // Apply answers to buttons
        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            if (i < q.answers.Length)
            {
                int answerIndex = indices[i];

                TMP_Text buttonText = AnswerButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText == null)
                {
                    Debug.LogError($"AnswerButton {i} is missing a TMP_Text component in its children!");
                    continue;
                }

                AnswerButtons[i].gameObject.SetActive(true);
                buttonText.text = q.answers[answerIndex];
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

        // Advance to the next question for the next time LoadQuestion is called.
        currentQuestionIndex++;
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