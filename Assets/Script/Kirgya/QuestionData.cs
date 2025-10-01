using UnityEngine;

// Add this line if you didn't create a separate QuestionEnums.cs script
public enum Subject { ComputerProgramming1, ComputerProgramming2, OOP, DataStructures }
public enum SchoolTerm { Prelim, Midterms, Prefinals, Finals }
public enum Difficulty { Novice, Advanced, Masters }

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
    public Subject subject;
    public SchoolTerm schoolTerm;
    public Difficulty difficulty;

    [TextArea(2, 5)]
    public string questionText;

    public string[] answers; // Multiple choices
    public int correctAnswerIndex; // Index of correct answer

    // For Identification
    public string correctAnswerText;

    // True/False
    public bool correctAnswerBool;

    // Matching
    public string[] prompts;
    public string[] choices;
}