using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AdvancedDungeonQuizManager : MonoBehaviour
{
    // --- UI References for Advanced Formats ---
    [Header("UI Panels")]
    public GameObject quizMasterPanel; // The main container panel for all quiz UIs
    public GameObject trueFalsePanel;
    public GameObject identificationPanel;

    [Header("Shared UI Components")]
    public TMP_Text QuestionText;
    public Image FeedbackFlash;

    [Header("True/False Components")]
    public Button trueButton;
    public Button falseButton;

    [Header("Identification Components")]
    public TMP_InputField identificationInput;
    public Button identificationSubmitButton;
    // ----------------------------------------------------

    // Retained References and State
    // NOTE: This should probably be AdvancedBattleManager, but keeping NoviceBattleManager 
    // for now as per your original code.
    public NoviceBattleManager battleManager; 
    private QuestionData currentQuestion; // Stores the currently loaded question data
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
        // Load all QuestionData assets
        masterQuestionPool = Resources.LoadAll<QuestionData>("").ToList();
        if (masterQuestionPool.Count == 0)
        {
            Debug.LogError("No QuestionData ScriptableObjects found in Resources folders! Quiz will not function.");
        }
    }

    private void Start()
    {
        // Initial UI Setup
        if (quizMasterPanel != null)
            quizMasterPanel.SetActive(false);
        if (FeedbackFlash != null)
            FeedbackFlash.gameObject.SetActive(false);

        // Assign listeners for the new buttons using the appropriate index (0=True, 1=False)
        // Ensure old listeners are cleared before adding new ones
        trueButton?.onClick.RemoveAllListeners();
        trueButton?.onClick.AddListener(() => OnTrueFalseSelected(0)); // 0 represents "True" 
        
        falseButton?.onClick.RemoveAllListeners();
        falseButton?.onClick.AddListener(() => OnTrueFalseSelected(1)); // 1 represents "False"
        
        identificationSubmitButton?.onClick.RemoveAllListeners();
        identificationSubmitButton?.onClick.AddListener(OnIdentificationSubmitted);
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
        if (quizMasterPanel != null)
            quizMasterPanel.SetActive(true);
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
        if (quizMasterPanel != null)
            quizMasterPanel.SetActive(false);
        trueFalsePanel?.SetActive(false);
        identificationPanel?.SetActive(false);
        StopAllCoroutines();
    }

    public int GetCorrectAnswers() => correctAnswers;
    public int GetWrongAnswers() => wrongAnswers;

    private void ShuffleActivePool()
    {
        if (activeQuestionPool == null) return;
        // Standard Fisher-Yates shuffle for full randomization (type and content)
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
        
        // Filter by all criteria AND ensure only True/False or Identification are included.
        activeQuestionPool = masterQuestionPool
          .Where(q =>
            q.difficulty == selectedDifficulty &&
            q.subject == selectedSubject &&
            q.schoolTerm == selectedTerm &&
            (q.type == QuestionType.TrueFalse || q.type == QuestionType.Identification)
          )
          .ToList();

        if (activeQuestionPool.Count == 0)
        {
            Debug.LogWarning($"No T/F or Identification questions found for the selected filters: {selectedSubject}, {selectedTerm}, {selectedDifficulty}.");
        }
        
        // Shuffle is called here to randomize the question types (ToF, Ident, ToF, Ident...)
        ShuffleActivePool(); 
    }

    private void LoadQuestion()
    {
        if (!isQuizActive || activeQuestionPool == null || activeQuestionPool.Count == 0)
        {
            Debug.LogError("Cannot load question: Quiz is inactive or question pool is empty.");
            EndQuiz();
            return;
        }

        // Cycle back to the start and reshuffle if we run out of questions
        if (currentQuestionIndex >= activeQuestionPool.Count)
        {
            ShuffleActivePool();
            currentQuestionIndex = 0;
        }
        
        currentQuestion = activeQuestionPool[currentQuestionIndex];
        QuestionText.text = currentQuestion.questionText;

        // Hide both specific panels before showing the correct one
        trueFalsePanel.SetActive(false);
        identificationPanel.SetActive(false);
        
        switch (currentQuestion.type)
        {
            case QuestionType.TrueFalse:
                trueFalsePanel.SetActive(true);
                break;
            
            case QuestionType.Identification:
                identificationPanel.SetActive(true);
                // Clear input field for new question
                identificationInput.text = "";
                break;

            default:
                Debug.LogError($"Unsupported QuestionType: {currentQuestion.type} loaded. Skipping.");
                currentQuestionIndex++;
                LoadQuestion();
                return;
        }
    }
    
    // --- Answer Evaluation for True/False (Uses correctAnswerIndex) ---
    private void OnTrueFalseSelected(int chosenIndex)
    {
        // Compare the button index (0 or 1) to the stored correct index
        bool correct = (chosenIndex == currentQuestion.correctAnswerIndex);
        
        // Hide only the sub-panel
        trueFalsePanel.SetActive(false); 
        
        StartCoroutine(ShowFeedbackAndContinue(correct));
    }
    
    // --- Answer Evaluation for Identification (Uses correctAnswerText) ---
    private void OnIdentificationSubmitted()
    {
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
        
        currentQuestionIndex++;
        
        LoadQuestion(); 
    }
}