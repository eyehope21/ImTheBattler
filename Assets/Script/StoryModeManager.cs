using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryModeManager : MonoBehaviour
{
   
    // Called when user clicks a subject
    public void SelectSubject(string subjectName)
    {
        PlayerPrefs.SetString("SelectedSubject", subjectName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Quarter");
    }

    // Called when user clicks a quarter (1-4)
    public void SelectQuarter(int quarterNumber)
    {
        PlayerPrefs.SetInt("SelectedQuarter", quarterNumber);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Chapters");
    }

    // Called when user clicks a chapter (1-3)
    public void SelectChapter(int chapterNumber)
    {
        PlayerPrefs.SetInt("SelectedChapter", chapterNumber);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Episodes");
    }

    
    public void SelectEpisode(int episodeNumber)
    {
        PlayerPrefs.SetInt("SelectedEpisode", episodeNumber);
        PlayerPrefs.Save();

        // Dynamically load the correct scene based on the episode number
        string sceneToLoad = "Episode";
        SceneManager.LoadScene(sceneToLoad);
    }

    // Optional: Back button
    public void GoBack(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
