using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // Required for Enum.Parse

public static class DungeonFilterLoader
{
    // Maps the PlayerPrefs subject string to the Subject enum
    public static Subject LoadSubject()
    {
        string subjectName = PlayerPrefs.GetString("SelectedSubject", "ComputerProgramming1"); // Default to CP1

        // This attempts to parse the string into the enum value
        if (Enum.TryParse(subjectName, true, out Subject subject))
        {
            return subject;
        }

        Debug.LogError($"Invalid Subject name '{subjectName}' found in PlayerPrefs. Defaulting to ComputerProgramming1.");
        return Subject.ComputerProgramming1;
    }

    // Maps the PlayerPrefs Quarter (1-4) to the SchoolTerm enum (Prelim=0, Midterms=1, etc.)
    public static SchoolTerm LoadSchoolTerm()
    {
        // Your SelectQuarter2 uses quarterNumber (1-4). We assume Prelim=1, Midterms=2, etc.
        int quarterNumber = PlayerPrefs.GetInt("SelectedQuarter", 1); // Default to 1 (Prelim)

        int termIndex = quarterNumber - 1; // Convert 1-based index to 0-based enum index

        if (Enum.IsDefined(typeof(SchoolTerm), termIndex))
        {
            return (SchoolTerm)termIndex;
        }

        Debug.LogError($"Invalid Quarter number {quarterNumber} found in PlayerPrefs. Defaulting to Prelim.");
        return SchoolTerm.Prelim;
    }

    // Maps the PlayerPrefs Portal/Difficulty (1-3) to the Difficulty enum (Novice=0, Advanced=1, Masters=2)
    public static Difficulty LoadDifficulty()
    {
        // Your SelectPortal uses an int (assuming 1=Novice, 2=Advanced, 3=Masters)
        int portalNumber = PlayerPrefs.GetInt("SelectedPortal", 1); // Default to 1 (Novice)

        int difficultyIndex = portalNumber - 1; // Convert 1-based index to 0-based enum index

        if (Enum.IsDefined(typeof(Difficulty), difficultyIndex))
        {
            return (Difficulty)difficultyIndex;
        }

        Debug.LogError($"Invalid Portal number {portalNumber} found in PlayerPrefs. Defaulting to Novice.");
        return Difficulty.Novice;
    }
}