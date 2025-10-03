using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CurrencyDisplay : MonoBehaviour
{
    public TMP_Text currencyText;

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (PlayerProfile.Instance != null && currencyText != null)
        {
            currencyText.text = PlayerProfile.Instance.Coins.ToString();
        }
        else
        {
            currencyText.text = "Error";
        }
    }
}