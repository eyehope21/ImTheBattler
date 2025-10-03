using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile Instance;

    public string Username { get; private set; }
    public int Level { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProfile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUsername(string username)
    {
        Username = username;
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();
    }

    public void InitializeLevel()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            Level = 1;
            PlayerPrefs.SetInt("Level", Level);
            PlayerPrefs.Save();
        }
        else
        {
            Level = PlayerPrefs.GetInt("Level");
        }
    }

    private void LoadProfile()
    {
        Username = PlayerPrefs.GetString("Username", "Guest");
        Level = PlayerPrefs.GetInt("Level", 1);
    }
}