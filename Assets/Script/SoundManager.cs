using UnityEngine.UI;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Slider masterVolumeSlider;
    private const string MasterVolumeKey = "masterVolume";

    void Start()
    {
        // Load the saved volume setting
        float savedVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f); // Default to 1 (full volume)
        AudioListener.volume = savedVolume;
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedVolume;
        }
    }

    public void OnMasterVolumeChanged(float newVolume)
    {
        // Set the global volume
        AudioListener.volume = newVolume;
        // Save the new volume setting
        PlayerPrefs.SetFloat(MasterVolumeKey, newVolume);
    }
}