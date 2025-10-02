using UnityEngine;

public enum Subject { ComputerProgramming1, ComputerProgramming2, OOP, DataStructures }
public enum SchoolTerm { Prelim, Midterms, Prefinals, Finals }
public enum Difficulty { Novice, Advanced, Masters }
public enum QuestionType{ MultipleChoice, Identification, TrueFalse, Matching }

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public class QuestionData : ScriptableObject
{
    public Subject subject;
    public SchoolTerm schoolTerm;
    public Difficulty difficulty;

    [TextArea(2, 5)]
    public string questionText;

    public string[] answers;
    public int correctAnswerIndex; 

    public string correctAnswerText;

    public bool correctAnswerBool;

    public string[] prompts;
    public string[] choices;
}