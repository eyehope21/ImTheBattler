using UnityEngine;

public enum QuestionType
{
    MultipleChoice,
    Identification,
    TrueFalse,
    Matching
}

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public class QuestionData : ScriptableObject
{
    [TextArea(2, 5)]
    public string questionText;

    public string[] answers; // Multiple choices
    public int correctAnswerIndex; // Index of correct answer

    // For Identification
    public string correctAnswerText; // <-- add this

    // True/False
    public bool correctAnswerBool;

    // Matching
    public string[] prompts;
    public string[] choices;
}