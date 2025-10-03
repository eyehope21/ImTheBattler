using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AdvancedDungeonQuizManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject quizMasterPanel; 
    public GameObject trueFalsePanel;
    public GameObject identificationPanel;

    [Header("Shared UI Components")]
    public TMP_Text QuestionText;
    public GameObject sharedQuestionTextPanel;
    public Image FeedbackFlash;

    [Header("Manager References")]
    public AdvancedBattleManager battleManager;

    [Header("True/False Components")]
    public Button trueButton;
    public Button falseButton;

    [Header("Identification Components")]
    public TMP_InputField identificationInput;
    public Button identificationSubmitButton;
    private QuestionData currentQuestion; 
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
            Debug.LogError("No QuestionData ScriptableObjects found in Resources folders! Advanced Quiz will not function.");
        }
    }

    private void Start()
    {
        // Set up button listeners that remain constant
        trueButton.onClick.AddListener(() => OnTrueFalseSelected(true));
        falseButton.onClick.AddListener(() => OnTrueFalseSelected(false));
        identificationSubmitButton.onClick.AddListener(OnIdentificationSubmitted);

        // Ensure all panels are hidden at start
        if (quizMasterPanel != null) quizMasterPanel.SetActive(false);
        if (trueFalsePanel != null) trueFalsePanel.SetActive(false);
        if (identificationPanel != null) identificationPanel.SetActive(false);
    }

    // --- Setup and Control Methods ---

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
        if (quizMasterPanel != null) quizMasterPanel.SetActive(true);
        if (FeedbackFlash != null) FeedbackFlash.gameObject.SetActive(false);
        if (sharedQuestionTextPanel != null) sharedQuestionTextPanel.SetActive(true);

        correctAnswers = 0;
        wrongAnswers = 0;
        currentQuestionIndex = 0;
        ShuffleActivePool();
        LoadQuestion();
    }

    public void EndQuiz()
    {
        isQuizActive = false;
        if (quizMasterPanel != null) quizMasterPanel.SetActive(false);
        if (sharedQuestionTextPanel != null) sharedQuestionTextPanel.SetActive(false);
        StopAllCoroutines();
    }

    public int GetCorrectAnswers() => correctAnswers;
    public int GetWrongAnswers() => wrongAnswers;

    private void ShuffleActivePool()
    {
        if (activeQuestionPool == null) return;
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
            return;
        }

        // 1. Filter by Difficulty (Advanced), Subject, and Term
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

            // FALLBACK: Try filtering by only Subject and Term
            activeQuestionPool = masterQuestionPool
                .Where(q => q.subject == selectedSubject && q.schoolTerm == selectedTerm)
                .ToList();

            if (activeQuestionPool.Count == 0)
            {
                Debug.LogError($"CRITICAL: Still no questions found after fallback.");
            }
        }

        ShuffleActivePool();
    }

    // --- Core Question Loading Logic ---

    private void HideAllQuestionPanels()
    {
        if (trueFalsePanel != null) trueFalsePanel.SetActive(false);
        if (identificationPanel != null) identificationPanel.SetActive(false);
        // Add other panels here (e.g., matchingPanel.SetActive(false);)
    }

    private void LoadQuestion()
    {
        if (!isQuizActive) return;
        if (activeQuestionPool == null || activeQuestionPool.Count == 0)
        {
            Debug.LogError("Quiz pool is empty! Cannot load question.");
            EndQuiz();
            return;
        }

        // --- Iterative Search for a Valid Question ---
        currentQuestion = null;
        int questionsToSearch = activeQuestionPool.Count * 2; // Safety limit

        for (int i = 0; i < questionsToSearch; i++)
        {
            // Handle index wrap and reshuffle
            if (currentQuestionIndex >= activeQuestionPool.Count)
            {
                ShuffleActivePool();
                currentQuestionIndex = 0;
                if (activeQuestionPool.Count == 0) break;
            }

            QuestionData q = activeQuestionPool[currentQuestionIndex];

            // Validation Check based on expected type (ToF or Identification)
            bool isValid = false;

            if (q.questionType == QuestionType.TrueFalse)
            {
                // ToF only needs questionText
                isValid = !string.IsNullOrWhiteSpace(q.questionText);
            }
            else if (q.questionType == QuestionType.Identification)
            {
                // Identification needs questionText AND correctAnswerText
                isValid = !string.IsNullOrWhiteSpace(q.questionText) && !string.IsNullOrWhiteSpace(q.correctAnswerText);
            }
            // Add checks for other types here

            if (!isValid)
            {
                // --- DETAILED LOGGING ---
                Debug.LogError($"Question '{q.name}' failed validation for type {q.questionType}. Skipping.");
                // ------------------------
                currentQuestionIndex++;
                continue; // Skip and check next question
            }

            // Valid question found!
            currentQuestion = q;
            break;
        }

        // Check 2: Did the search fail?
        if (currentQuestion == null)
        {
            Debug.LogError("Exhausted all questions in the pool. None were valid or of supported type. Ending quiz.");
            EndQuiz();
            return;
        }

        // UI Setup
        HideAllQuestionPanels();
        QuestionText.text = currentQuestion.questionText;

        // Dynamic Panel Switching
        switch (currentQuestion.questionType)
        {
            case QuestionType.TrueFalse:
                trueFalsePanel.SetActive(true);
                break;

            case QuestionType.Identification:
                identificationPanel.SetActive(true);
                // Clear input field for fresh start
                identificationInput.text = "";
                break;

            default:
                Debug.LogError($"Unsupported QuestionType: {currentQuestion.questionType}. Skipping question.");
                currentQuestionIndex++;
                LoadQuestion();
                return;
        }

        // Advance the index for the next call
        currentQuestionIndex++;
    }

    // --- Answer Evaluation Logic ---

    private void OnTrueFalseSelected(bool selectedAnswer)
    {
        if (!isQuizActive) return;

        bool correct = selectedAnswer == currentQuestion.correctAnswerBool;

        // Hide only the sub-panel (the master panel remains visible)
        trueFalsePanel.SetActive(false);

        StartCoroutine(ShowFeedbackAndContinue(correct));
    }

    private void OnIdentificationSubmitted()
    {
        if (!isQuizActive) return;

        string submittedText = identificationInput.text;

        // Use case-insensitive and trimmed comparison for flexible answering
        bool correct = string.Equals(
            submittedText.Trim(),
            currentQuestion.correctAnswerText.Trim(),
            StringComparison.OrdinalIgnoreCase
        );

        // Hide only the sub-panel
        identificationPanel.SetActive(false);

        StartCoroutine(ShowFeedbackAndContinue(correct));
    }

    // --- Feedback and Progression ---

    private IEnumerator ShowFeedbackAndContinue(bool isCorrect)
    {
        OnAnswerEvaluated?.Invoke(isCorrect);

        if (isCorrect)
            correctAnswers++;
        else
            wrongAnswers++;

        // Only flash the screen for wrong answers (damage feedback)
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

        // Wait period before showing the next question
        yield return new WaitForSeconds(1f);

        if (!isQuizActive)
        {
            yield break;
        }

        // NOTE: LoadQuestion already increments the index internally if a valid question is loaded.
        // We only call it here to load the next question.

        // Ensure master panel is still visible (it was hidden inside OnAnswerSelected for True/False)
        if (quizMasterPanel != null) quizMasterPanel.SetActive(true);

        LoadQuestion();
    }
}