using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchingTypeManager : MonoBehaviour
{
    [Header("Questions")]
    public MatchingQuestionData[] questions;   // All matching questions
    private List<MatchingQuestionData> questionPool;
    private int currentQuestionIndex = 0;

    [Header("UI References")]
    public DropZone[] dropZones;           // Left panel drop zones
    public DraggableItem[] draggableItems; // Right panel draggable answers
    public TMP_Text feedbackText;          // Partial credit feedback

    void Start()
    {
        ResetQuestionPool();
        LoadQuestion();
    }

    void ResetQuestionPool()
    {
        questionPool = new List<MatchingQuestionData>(questions);

        // Shuffle questions (Fisher-Yates)
        for (int i = 0; i < questionPool.Count; i++)
        {
            int rnd = Random.Range(i, questionPool.Count);
            (questionPool[i], questionPool[rnd]) = (questionPool[rnd], questionPool[i]);
        }

        currentQuestionIndex = 0;
    }

    void LoadQuestion()
    {
        if (questionPool.Count == 0)
        {
            Debug.LogWarning("No matching questions in pool!");
            return;
        }

        if (currentQuestionIndex >= questionPool.Count)
        {
            ResetQuestionPool();
        }

        currentQuestionIndex = Mathf.Clamp(currentQuestionIndex, 0, questionPool.Count - 1);
        MatchingQuestionData q = questionPool[currentQuestionIndex];

        feedbackText.text = "";

        int pairCount = Mathf.Min(dropZones.Length, draggableItems.Length, q.prompts.Length, q.choices.Length);

        // Assign draggable items (shuffle choices)
        List<int> indices = new List<int>();
        for (int i = 0; i < pairCount; i++) indices.Add(i);
        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = Random.Range(i, indices.Count);
            (indices[i], indices[rnd]) = (indices[rnd], indices[i]);
        }

        for (int i = 0; i < draggableItems.Length; i++)
        {
            if (i < pairCount)
            {
                draggableItems[i].gameObject.SetActive(true);
                draggableItems[i].name = q.choices[indices[i]];
                draggableItems[i].ReturnToOriginal();

                if (draggableItems[i].itemText != null)
                {
                    draggableItems[i].itemText.text = q.choices[indices[i]];
                }
                else
                {
                    Debug.LogWarning($"Draggable {i} has no TMP_Text assigned!");
                }
            }
            else
            {
                draggableItems[i].gameObject.SetActive(false);
            }
        }

        // Assign drop zones (left panel prompts)
        // Assign DropZones correctAnswerID and set TMP text
        for (int i = 0; i < dropZones.Length; i++)
        {
            if (i < pairCount)
            {
                dropZones[i].gameObject.SetActive(true);
                dropZones[i].correctAnswerID = q.choices[i];

                // Find TMP_Text in the drop zone
                TMP_Text dzText = dropZones[i].GetComponentInChildren<TMP_Text>();
                if (dzText != null)
                {
                    dzText.text = q.prompts[i];
                }
                else
                {
                    Debug.LogWarning($"DropZone {dropZones[i].name} has no TMP_Text child!");
                }
            }
            else
            {
                dropZones[i].gameObject.SetActive(false);
            }
        }
    }

    public void CheckMatches()
    {
        int correctCount = 0;
        int activeDropZones = 0;

        foreach (var dz in dropZones)
        {
            if (!dz.gameObject.activeSelf) continue;
            activeDropZones++;

            if (dz.transform.childCount > 0)
            {
                DraggableItem item = dz.transform.GetChild(0).GetComponent<DraggableItem>();
                if (item != null && item.name == dz.correctAnswerID)
                {
                    correctCount++;
                }
            }
        }

        feedbackText.text = $"{correctCount} / {activeDropZones} correct!";

        if (correctCount == activeDropZones && activeDropZones > 0)
        {
            currentQuestionIndex++;
            Invoke("LoadQuestion", 1.5f);
        }
    }
}