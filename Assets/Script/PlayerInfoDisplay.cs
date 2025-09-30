using UnityEngine;
using TMPro;

public class PlayerInfoDisplay : MonoBehaviour
{
    public TMP_Text infoText;

    void Start()
    {
        // This is a more robust way to ensure PlayerProfile is loaded
        if (PlayerProfile.Instance != null)
        {
            infoText.text = $"Lv.{PlayerProfile.Instance.Level}  {PlayerProfile.Instance.Username}";
        }
        else
        {
            infoText.text = "";
        }
    }
}