using UnityEngine;
public enum Subject { ComputerProgramming1, ComputerProgramming2, OOP, DataStructures }
public enum SchoolTerm { Prelim, Midterms, Prefinals, Finals }
public enum Difficulty { Novice, Advanced, Masters }
public enum QuestionType { MultipleChoice, Identification, TrueFalse, Matching }

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public class QuestionData : ScriptableObject
{
    public Subject subject;
    public SchoolTerm schoolTerm;
    public Difficulty difficulty;
    public QuestionType type;

    [TextArea(2, 5)]
    public string questionText;

    public string[] answers;
    public int correctAnswerIndex; 

    public string correctAnswerText;

    public bool correctAnswerBool;

    [Header("Matching Data (Only used if Type is Matching)")]
    [Tooltip("The items displayed on the LEFT (Terms). Must be the correct match for the corresponding Choices[i].")]
    public string[] prompts; // Left Column: Terms/Questions

    [Tooltip("The items displayed on the RIGHT (Definitions). Must be the correct match for the corresponding Prompts[i].")]
    public string[] choices; // Right Column: Definitions/Answers

    public bool IsMatchingDataValid()
    {
        if (type != QuestionType.Matching) return false;
        if (prompts == null || choices == null) return false;
        return prompts.Length == choices.Length && prompts.Length > 0;
    }
}