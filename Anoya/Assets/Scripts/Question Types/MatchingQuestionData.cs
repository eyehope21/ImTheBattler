using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Matching Question", menuName = "Quiz/Matching")]
public class MatchingQuestionData : ScriptableObject
{
    public string[] prompts;  // Left panel questions
    public string[] choices;  // Right panel answers
}