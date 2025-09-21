using UnityEngine;
using TMPro;

public class PlayerInfoDisplay : MonoBehaviour
{
    public TMP_Text infoText;

    void Update()
    {
        if (PlayerProfile.Instance != null)
        {
            infoText.text = PlayerProfile.Instance.Username + "  Lv." + PlayerProfile.Instance.Level;
        }
    }
}
