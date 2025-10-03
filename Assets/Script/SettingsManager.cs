using Firebase.Auth;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public Slider soundSlider;
    public Slider musicSlider;

    private void Start()
    {
        // Load the saved volume setting
        float savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        SetMusicVolume(savedMusicVolume);
        SetSoundVolume(savedSoundVolume);

        soundSlider.value = savedSoundVolume;
        musicSlider.value = savedMusicVolume;
    }

    public void SetSoundVolume(float newVolume)
    {
        masterMixer.SetFloat("SoundVolume", newVolume);
        PlayerPrefs.SetFloat("SoundVolume", newVolume);
    }

    public void SetMusicVolume(float newVolume)
    {
        masterMixer.SetFloat("MusicVolume", newVolume);
        PlayerPrefs.SetFloat("MusicVolume", newVolume);
    }

    // Call this method when the SignOut button is clicked
    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        PlayerPrefs.SetInt("IsLoggedIn", 0);
        SceneManager.LoadScene("Login");
    }

    // Call this method when the Quit button is clicked
    public void QuitGame()
    {
        Application.Quit();
    }


    // Call this method when the Back button is clicked
    public void GoBack()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "Menu");
        SceneManager.LoadScene(previousScene);
    }
}