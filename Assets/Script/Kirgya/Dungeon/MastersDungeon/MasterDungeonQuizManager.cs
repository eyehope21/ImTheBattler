using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System;

// Ensure this class is either in its own file or at the top of this file, 
// and MUST have [System.Serializable].
[System.Serializable]
public class MatchPair
{
    public string Term;
    public string Definition;
    [HideInInspector] public bool IsMatched = false;
}

public class MasterDungeonQuizManager : MonoBehaviour
{
    // --- Public Events for Dungeon Manager Communication ---
    public event Action<bool> OnAnswerEvaluated;
    public event Action OnQuizComplete;

    // --- Panel Containers ---
    [Tooltip("Container for the Matching Quiz buttons/UI.")]
    public GameObject matchingQuizPanel;

    [Tooltip("Container for the Identification Quiz input/UI.")]
    public GameObject identificationQuizPanel;

    // IMPROVEMENT: Renamed for clarity in the Inspector
    [Tooltip("Panel containing the QUESTION TEXT (e.g., for Identification/TrueFalse)")]
    public GameObject identificationQuestionPanel;

    // --- Matching UI Elements (if Type == Matching) ---
    [Header("Matching UI")]
    [Tooltip("Drag the 3 Term Buttons from the left column here.")]
    public Button[] TermButtons;
    [Tooltip("Drag the 3 Definition Buttons from the right column here.")]
    public Button[] DefinitionButtons;

    // --- Identification UI Elements (if Type == Identification) ---
    [Header("Identification UI")]
    [Tooltip("The Text component to display the question for Identification.")]
    public TextMeshProUGUI identificationQuestionText;
    public TMP_InputField answerInputField;
    public Button submitButton;

    // --- Private State Variables ---

    // FIX: Added [SerializeField] to resolve the 'ExtensionOfNativeClass' error
    [SerializeField]
    private List<MatchPair> AllPairs = new List<MatchPair>();

    private List<int> termDataIndices;
    private int selectedTermDataIndex = -1;
    private int selectedDefinitionButtonIndex = -1;

    // NEW: Internal reference for the current question being played
    private QuestionData currentQuizData;

    // NEW: List to hold the currently available questions for the run (filter results)
    private List<QuestionData> availableQuestions;

    // NEW: Variables to store the current dungeon filters
    private Subject currentSubject;
    private SchoolTerm currentTerm;
    private Difficulty currentDifficulty;
    private bool isCurrentlyIdentification = false;

    // Quiz Tracking
    private int correctMatches = 0;
    private int wrongAttempts = 0;
    private string correctIDAnswer;

    // --- Public Control Methods ---

    /// <summary>
    /// Called by the Dungeon Manager to set filters and load the question pool.
    /// </summary>
    public void SetDungeonFilters(Subject subject, SchoolTerm term, Difficulty difficulty)
    {
        currentSubject = subject;
        currentTerm = term;
        currentDifficulty = difficulty;

        // Load the available pool of questions based on the filters
        LoadAvailableQuestions();

        Debug.Log($"Master Quiz Manager initialized for Subject: {subject}, Term: {term}, Difficulty: {difficulty}. Loaded {availableQuestions.Count} questions.");
    }

    /// <summary>
    /// Finds the next question in the pool and initializes the appropriate UI.
    /// </summary>
    public void StartQuiz()
    {
        // 1. Get the next question from the pool
        currentQuizData = GetNextQuizData();

        if (currentQuizData == null)
        {
            Debug.LogWarning("No more unique questions left for this dungeon run! Quiz Completed.");
            OnQuizComplete?.Invoke();
            return;
        }

        // 2. Hide all quiz panels initially
        HideAllQuizPanels();

        if (currentQuizData.questionType == QuestionType.Matching)
        {
            if (TermButtons.Length != DefinitionButtons.Length)
            {
                Debug.LogError("Quiz Manager Setup Error: The number of Matching Term Buttons must match the Definition Buttons.");
                return;
            }
            if (LoadMatchingData())
            {
                isCurrentlyIdentification = false;
                InitializeMatchingQuiz();
                matchingQuizPanel.SetActive(true);
            }
        }
        else if (currentQuizData.questionType == QuestionType.Identification)
        {
            isCurrentlyIdentification = true;
            InitializeIdentificationQuiz();
            identificationQuizPanel.SetActive(true);
            if (identificationQuestionPanel != null) identificationQuestionPanel.SetActive(true);
        }
        else
        {
            Debug.LogError($"Unsupported QuestionType: {currentQuizData.questionType}. Cannot start quiz.");
        }
    }

    public void EndQuiz()
    {
        HideAllQuizPanels();

        // Clear selection state
        selectedTermDataIndex = -1;
        selectedDefinitionButtonIndex = -1;

        // Reset counters
        correctMatches = 0;
        wrongAttempts = 0;
    }

    public int GetCorrectAnswers() => correctMatches;
    public int GetWrongAnswers() => wrongAttempts;

    // --- Data Loading & Setup ---

    private void LoadAvailableQuestions()
    {
        // 1. Construct the path string based on the current filter enums
        // Assumes resources are structured like: Resources/Quizzes/Subject/SchoolTerm/Difficulty/
        string path = "Quizzes/" + currentSubject.ToString() + "/" +
                      currentTerm.ToString() + "/" +
                      currentDifficulty.ToString();

        // 2. Use Resources.LoadAll to get all assets of type QuestionData in that folder
        QuestionData[] questionsArray = Resources.LoadAll<QuestionData>(path);

        if (questionsArray.Length == 0)
        {
            Debug.LogError($"No QuestionData assets found at path: {path}. Check your Resources folder structure!");
            availableQuestions = new List<QuestionData>();
            return;
        }

        // 3. Convert to list, shuffle, and store as the available pool
        availableQuestions = questionsArray.ToList();
        availableQuestions = availableQuestions.OrderBy(a => UnityEngine.Random.value).ToList();
    }

    private QuestionData GetNextQuizData()
    {
        if (availableQuestions == null || availableQuestions.Count == 0)
        {
            return null;
        }

        // Grab the first question, then remove it from the pool so it's not reused.
        QuestionData nextQuestion = availableQuestions[0];
        availableQuestions.RemoveAt(0);
        return nextQuestion;
    }

    private void HideAllQuizPanels()
    {
        matchingQuizPanel.SetActive(false);
        identificationQuizPanel.SetActive(false);
        if (identificationQuestionPanel != null) identificationQuestionPanel.SetActive(false);
    }

    private TextMeshProUGUI GetButtonText(Button button)
    {
        if (button == null) return null;
        return button.GetComponentInChildren<TextMeshProUGUI>();
    }

    // ==============================================================================
    // --- MATCHING QUIZ LOGIC ---
    // ==============================================================================

    private bool LoadMatchingData()
    {
        // Uses currentQuizData
        if (!currentQuizData.IsMatchingDataValid())
        {
            Debug.LogError("Matching Quiz Failed: Matching data arrays are invalid.");
            return false;
        }

        if (currentQuizData.prompts.Length != TermButtons.Length)
        {
            Debug.LogError($"Matching Data Mismatch: Data has {currentQuizData.prompts.Length} pairs, but you assigned {TermButtons.Length} buttons.");
            return false;
        }

        AllPairs.Clear();
        for (int i = 0; i < currentQuizData.prompts.Length; i++)
        {
            AllPairs.Add(new MatchPair
            {
                Term = currentQuizData.prompts[i],
                Definition = currentQuizData.choices[i],
                IsMatched = false
            });
        }
        return true;
    }

    private void InitializeMatchingQuiz()
    {
        correctMatches = 0;
        wrongAttempts = 0;
        selectedTermDataIndex = -1;
        selectedDefinitionButtonIndex = -1;

        // 1. Randomize the presentation order for the Terms column
        termDataIndices = Enumerable.Range(0, AllPairs.Count).ToList();
        termDataIndices = termDataIndices.OrderBy(a => UnityEngine.Random.value).ToList();

        // 2. Prepare the Definitions list (shuffling them separately)
        List<string> shuffledDefinitions = AllPairs.Select(p => p.Definition).ToList();
        shuffledDefinitions = shuffledDefinitions.OrderBy(a => UnityEngine.Random.value).ToList();

        // 3. Assign text and listeners to buttons
        for (int i = 0; i < AllPairs.Count; i++)
        {
            int buttonIndex = i;
            int dataIndex = termDataIndices[i];

            AllPairs[dataIndex].IsMatched = false;
            TermButtons[buttonIndex].interactable = true;
            DefinitionButtons[buttonIndex].interactable = true;

            // --- TERM BUTTONS (Left Column) ---
            GetButtonText(TermButtons[buttonIndex]).text = AllPairs[dataIndex].Term;
            TermButtons[buttonIndex].onClick.RemoveAllListeners();
            TermButtons[buttonIndex].onClick.AddListener(() => SelectTerm(dataIndex));
            TermButtons[buttonIndex].image.color = Color.white;

            // --- DEFINITION BUTTONS (Right Column) ---
            GetButtonText(DefinitionButtons[buttonIndex]).text = shuffledDefinitions[buttonIndex];
            DefinitionButtons[buttonIndex].onClick.RemoveAllListeners();
            DefinitionButtons[buttonIndex].onClick.AddListener(() => SelectDefinition(buttonIndex));
            DefinitionButtons[buttonIndex].image.color = Color.white;
        }
    }

    public void SelectTerm(int dataIndex)
    {
        // Deselection logic
        if (selectedTermDataIndex != -1)
        {
            Button prevButton = TermButtons.FirstOrDefault(b => GetButtonText(b).text == AllPairs[selectedTermDataIndex].Term);
            if (prevButton != null) prevButton.image.color = Color.white;
        }

        // Selection logic
        Button clickedButton = TermButtons.FirstOrDefault(b => GetButtonText(b).text == AllPairs[dataIndex].Term);
        if (clickedButton == null || !clickedButton.interactable) return;

        selectedTermDataIndex = dataIndex;
        clickedButton.image.color = Color.yellow;

        CheckMatch();
    }

    public void SelectDefinition(int buttonIndex)
    {
        // Deselection logic
        if (selectedDefinitionButtonIndex != -1)
        {
            DefinitionButtons[selectedDefinitionButtonIndex].image.color = Color.white;
        }

        // Selection logic
        if (!DefinitionButtons[buttonIndex].interactable) return;

        selectedDefinitionButtonIndex = buttonIndex;
        DefinitionButtons[buttonIndex].image.color = Color.yellow;

        CheckMatch();
    }

    private void CheckMatch()
    {
        if (selectedTermDataIndex == -1 || selectedDefinitionButtonIndex == -1) return;

        string correctDefinition = AllPairs[selectedTermDataIndex].Definition;
        string selectedDefinition = GetButtonText(DefinitionButtons[selectedDefinitionButtonIndex]).text;

        Button termButtonToManipulate = TermButtons.FirstOrDefault(b => GetButtonText(b).text == AllPairs[selectedTermDataIndex].Term);

        if (correctDefinition == selectedDefinition)
        {
            correctMatches++;
            OnAnswerEvaluated?.Invoke(true);

            if (termButtonToManipulate != null) termButtonToManipulate.interactable = false;
            DefinitionButtons[selectedDefinitionButtonIndex].interactable = false;

            if (termButtonToManipulate != null) termButtonToManipulate.image.color = Color.green;
            DefinitionButtons[selectedDefinitionButtonIndex].image.color = Color.green;

            AllPairs[selectedTermDataIndex].IsMatched = true;

            if (AllPairs.All(p => p.IsMatched))
            {
                OnQuizComplete?.Invoke();
            }
        }
        else
        {
            wrongAttempts++;
            OnAnswerEvaluated?.Invoke(false);

            if (termButtonToManipulate != null)
            {
                StartCoroutine(ResetColors(termButtonToManipulate, DefinitionButtons[selectedDefinitionButtonIndex]));
            }
        }

        selectedTermDataIndex = -1;
        selectedDefinitionButtonIndex = -1;
    }

    private IEnumerator ResetColors(Button termBtn, Button defBtn)
    {
        if (termBtn != null && termBtn.interactable) termBtn.image.color = Color.red;
        if (defBtn != null && defBtn.interactable) defBtn.image.color = Color.red;

        yield return new WaitForSeconds(0.5f);

        if (termBtn != null && termBtn.interactable) termBtn.image.color = Color.white;
        if (defBtn != null && defBtn.interactable) defBtn.image.color = Color.white;
    }

    // ==============================================================================
    // --- IDENTIFICATION QUIZ LOGIC ---
    // ==============================================================================

    private void InitializeIdentificationQuiz()
    {
        // Uses currentQuizData
        if (identificationQuestionText != null) identificationQuestionText.text = currentQuizData.questionText;
        if (answerInputField != null) answerInputField.text = "";

        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(SubmitIdentificationAnswer);
            submitButton.interactable = true;
        }

        correctIDAnswer = currentQuizData.correctAnswerText.Trim().ToUpper();
    }

    public void SubmitIdentificationAnswer()
    {
        if (!isCurrentlyIdentification || answerInputField == null || submitButton == null) return;

        string submittedAnswer = answerInputField.text.Trim().ToUpper();

        if (submittedAnswer == correctIDAnswer)
        {
            correctMatches++;
            OnAnswerEvaluated?.Invoke(true);
            submitButton.interactable = false;

            // Immediately complete the quiz, as ID is usually a single question
            OnQuizComplete?.Invoke();
        }
        else
        {
            wrongAttempts++;
            OnAnswerEvaluated?.Invoke(false);

            StartCoroutine(FlashInputRed(answerInputField));
        }
    }

    private IEnumerator FlashInputRed(TMP_InputField inputField)
    {
        Color originalColor = inputField.image.color;
        inputField.image.color = Color.red;
        inputField.text = ""; // Clear the wrong answer
        yield return new WaitForSeconds(0.5f);
        inputField.image.color = originalColor;
    }
}