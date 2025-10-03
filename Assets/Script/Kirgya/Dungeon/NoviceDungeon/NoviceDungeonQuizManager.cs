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
            Debug.LogWarning($"No questions found for the strict filters: {selectedSubject}, {selectedTerm}, {selectedDifficulty}.");

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
                Debug.LogWarning("Fallback activated: Loaded questions by Subject and Term only. Check Difficulty enum consistency.");
            }
        }

        ShuffleActivePool();
    }

    private void LoadQuestion()
    {
        if (!isQuizActive || activeQuestionPool == null || activeQuestionPool.Count == 0)
        {
            Debug.LogError("Cannot load question: Quiz is inactive or question pool is empty.");
            return;
        }

        if (currentQuestionIndex >= activeQuestionPool.Count)
        {
            ShuffleActivePool();
            currentQuestionIndex = 0;
        }
        QuestionData q = activeQuestionPool[currentQuestionIndex];
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