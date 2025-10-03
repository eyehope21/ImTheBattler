using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using System.Threading.Tasks;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile Instance;

    public string Username { get; private set; }
    public int Level { get; private set; }
    public int Coins { get; private set; }

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
        Coins = PlayerPrefs.GetInt("Coins", 500);
    }


    // ✅ New method to initialize coins for a new player
    public void InitializeCoins()
    {
        if (!PlayerPrefs.HasKey("Coins"))
        {
            Coins = 500;
            PlayerPrefs.SetInt("Coins", Coins);
            PlayerPrefs.Save();
        }
        else
        {
            Coins = PlayerPrefs.GetInt("Coins");
        }
    }

    // ✅ New method to add coins
    public void AddCoins(int amount)
    {
        Coins += amount;
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.Save();
    }

    // ✅ New method to spend coins
    public bool TrySpendCoins(int cost)
    {
        if (Coins >= cost)
        {
            Coins -= cost;
            PlayerPrefs.SetInt("Coins", Coins);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}
