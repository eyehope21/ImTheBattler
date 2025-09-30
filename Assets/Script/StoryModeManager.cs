using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryModeManager : MonoBehaviour
{
   
    // Called when user clicks a subject
    public void SelectSubject(string subjectName)
    {
        PlayerPrefs.SetString("SelectedSubject", subjectName);
        PlayerPrefs.Save();
        SceneHistory.LoadScene("Quarter");
    }

    // Called when user clicks a quarter (1-4)
    public void SelectQuarter(int quarterNumber)
    {
        PlayerPrefs.SetInt("SelectedQuarter", quarterNumber);
        PlayerPrefs.Save();
        SceneHistory.LoadScene("Chapters");
    }

    // Called when user clicks a chapter (1-3)
    public void SelectChapter(int chapterNumber)
    {
        PlayerPrefs.SetInt("SelectedChapter", chapterNumber);
        PlayerPrefs.Save();
        SceneHistory.LoadScene("Episodes");
    }


    public void SelectEpisode(int episodeNumber)
    {
        PlayerPrefs.SetInt("SelectedEpisode", episodeNumber);
        PlayerPrefs.Save();

        // Dynamically load the correct scene based on the episode number
        string sceneToLoad = "Episode";
        SceneHistory.LoadScene(sceneToLoad);
    }

    

    // Optional: Back button
    public void GoBack(string sceneName)
    {
        SceneHistory.LoadScene(sceneName);
    }

    public void GoToMenu()
    {
        SceneHistory.LoadScene("Menu");
    }
}
