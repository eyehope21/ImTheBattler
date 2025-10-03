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

    [TextArea(2, 5)]
    public string questionText; // USED by Novice Quiz

    public string[] answers; // USED by Novice Quiz
    public int correctAnswerIndex; // USED by Novice Quiz 

    public string correctAnswerText; // UNUSED by Novice MC
    public bool correctAnswerBool; // UNUSED by Novice MC

    public string[] prompts; // UNUSED by Novice MC
    public string[] choices; // UNUSED by Novice MC
}