using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyJoiner : MonoBehaviour
{
    public MultiplayerManager multiplayerManager;
    public GameObject joinLobbyPopup;
    public TMP_InputField joinCodeInput;

    public void ShowJoinPopup()
    {
        joinLobbyPopup.SetActive(true);
    }

    public void JoinLobby()
    {
        string hostCode = joinCodeInput.text;
        if (!string.IsNullOrEmpty(hostCode))
        {
            multiplayerManager.JoinBattle(hostCode);
            joinLobbyPopup.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Please enter a host code.");
        }
    }
}