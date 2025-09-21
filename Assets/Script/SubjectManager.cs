using UnityEngine;
using UnityEngine.SceneManagement;

public class SubjectManager : MonoBehaviour
{
    // Called when user clicks a subject
    public void SelectSubject(string subjectName)
    {
        PlayerPrefs.SetString("SelectedSubject", subjectName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Quarter 2");
    }

    // Called when user clicks a quarter (1-4)
    public void SelectQuarter2(int quarterNumber)
    {
        PlayerPrefs.SetInt("SelectedQuarter", quarterNumber);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Difficulty");
    }

    //Quizgame
    public void SelectDifficulty(int PortalNumber)
    {
        PlayerPrefs.SetInt("SelectedPortal", PortalNumber);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Portal");
    }
    public void SelectPortal(int Quizgame)
    {
        PlayerPrefs.SetInt("SelectedPortal", Quizgame);
        PlayerPrefs.Save();
        SceneManager.LoadScene("QuizGame");
    }


    public void SelectEpisode(int episodeNumber)
    {
        PlayerPrefs.SetInt("SelectedEpisode", episodeNumber);
        PlayerPrefs.Save();

        // Dynamically load the correct scene based on the episode number
        string sceneToLoad = "Episode" + episodeNumber + 1;
        SceneManager.LoadScene(sceneToLoad);
    }

    // Optional: Back button
    public void GoBack(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
