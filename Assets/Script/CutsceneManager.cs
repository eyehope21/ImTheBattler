using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI playerDialogueText;
    public TextMeshProUGUI bitDialogueText;
    public TextMeshProUGUI questionText;
    public GameObject tapToContinueUI;
    public GameObject puzzleUI;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.03f;
    public float minVisibleTime = 0.2f;

    [Header("Scene Settings")]
    public GameObject bitCharacter;
    public GameObject blockadeObject;
    public string nextSceneName = "Episodes";

    // Internal state
    private Coroutine pulseRoutine;

    void Start()
    {
        puzzleUI.SetActive(false);
        bitCharacter.SetActive(false);
        if (blockadeObject != null) blockadeObject.SetActive(true);
        if (tapToContinueUI != null) tapToContinueUI.SetActive(false);

        playerDialogueText.text = "";
        bitDialogueText.text = "";
        questionText.text = "";

        StartCoroutine(PlayIntro());
    }

    IEnumerator TypeAndWait(TextMeshProUGUI box, string message, TextMeshProUGUI clearOtherBox = null)
    {
        if (clearOtherBox != null) clearOtherBox.text = "";

        if (box == null) yield break;
        if (tapToContinueUI != null) tapToContinueUI.SetActive(false);

        yield return new WaitUntil(() => !Input.GetMouseButton(0));

        box.text = "";
        foreach (char ch in message)
        {
            if (Input.GetMouseButtonDown(0))
            {
                box.text = message;
                break;
            }
            box.text += ch;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(minVisibleTime);

        if (tapToContinueUI != null)
        {
            tapToContinueUI.SetActive(true);
        }

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        if (tapToContinueUI != null)
        {
            tapToContinueUI.SetActive(false);
        }
    }

    IEnumerator PlayIntro()
    {
        yield return StartCoroutine(TypeAndWait(playerDialogueText, "(Scene fades in… birds chirping… forest around.)"));

        if (bitCharacter != null) bitCharacter.SetActive(true);
        yield return StartCoroutine(TypeAndWait(bitDialogueText, "Bit: 'Welcome to Code World. I am Bit, your guide.'", playerDialogueText));
        yield return StartCoroutine(TypeAndWait(bitDialogueText, "Bit: 'See that blockade ahead? You need to solve a puzzle to pass.'", playerDialogueText));

        puzzleUI.SetActive(true);
        questionText.text = "Which of the following is a high-level programming language?";
    }

    public void OnCorrectAnswer()
    {
        puzzleUI.SetActive(false);
        if (blockadeObject != null) blockadeObject.SetActive(false);

        StartCoroutine(PostPuzzleSequence());
    }

    public void OnWrongAnswer()
    {
        
        StartCoroutine(TypeAndWait(bitDialogueText, "Bit: Wow...... You can try again if you like.", playerDialogueText));
    }

    IEnumerator PostPuzzleSequence()
    {
        yield return StartCoroutine(TypeAndWait(bitDialogueText, "Bit: 'Well done. The path is clear.'", playerDialogueText));

        SceneManager.LoadScene("Episodes");
    }
}