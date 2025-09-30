using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    [Header("Totems")]
    public Totem[] totems;
    private Totem firstChoice;
    private Totem secondChoice;

    [Header("Puzzle UI")]
    public TextMeshProUGUI messageText; // puzzle instructions & results

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText; // Bit/Player conversation
    public Image bitPortrait;

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f;

    private string[] lines;
    private int index;
    private bool isTyping;
    private Coroutine typingCoroutine;

    private bool puzzleActive = false; // lock puzzle until dialogue is done

    void Start()
    {
        dialoguePanel.SetActive(false);
        messageText.text = "";

        // Intro dialogue
        lines = new string[]
        {
            "Player: Where… am I?",
            "Player: Hello?",
            "Bit: I am Bit, your guide. This place is the Cave of Codes—here, your journey starts here.",
            "Player: Why am I here?",
            "Bit: Before you stand four totems. Two hold the truth, two hold falsehoods. Choose wisely.",
            "Bit: If you choose right, the totems will shine and the way forward will open. If not… their light fades, and you must try again.",
            "Player: Truth about what?",
            "Bit: The oldest languages of the machine. At the lowest level, a computer understands only simple instructions — patterns of ones and zeros. This is called Machine Language. Strong, but very hard for people to use.",
            "Bit: To make it easier, another form was made — one that uses short words and symbols instead of long strings of numbers. This is Assembly Language. Still close to the machine, but easier for humans to grasp.",
            "Player: Alright… let’s see if I can figure this out."
        };

        StartDialogue(lines);
    }

    // ==========================
    // DIALOGUE SYSTEM
    // ==========================
    public void StartDialogue(string[] dialogueLines)
    {
        lines = dialogueLines;
        index = 0;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        string line = lines[index];
        string[] parts = line.Split(':');
        string speaker = parts[0].Trim();

        // Show Bit portrait only if Bit is speaking
        bitPortrait.gameObject.SetActive(speaker == "Bit");

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line)); // show full line (speaker + message)
    }

    IEnumerator TypeText(string fullLine)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in fullLine.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = lines[index]; // instantly show full line
                isTyping = false;
            }
            else
            {
                index++;
                if (index < lines.Length)
                {
                    ShowLine();
                }
                else
                {
                    dialoguePanel.SetActive(false);
                    if (!puzzleActive) // start puzzle after intro dialogue
                    {
                        puzzleActive = true;
                        messageText.text = "Choose 2 totems that represent low-level languages.";
                    }
                }
            }
        }
    }

    // ==========================
    // PUZZLE SYSTEM
    // ==========================
    public void OnTotemChosen(Totem totem)
    {
        if (!puzzleActive) return; // block until dialogue done

        // Show description in PUZZLE UI only
        if (messageText != null)
            messageText.text = $"{totem.totemName}: {totem.description}";

        if (firstChoice == null)
        {
            firstChoice = totem;
            firstChoice.SetGlow(true);
        }
        else if (secondChoice == null && totem != firstChoice)
        {
            secondChoice = totem;
            secondChoice.SetGlow(true);

            StartCoroutine(CheckChoices());
        }
    }

    IEnumerator CheckChoices()
    {
        yield return new WaitForSeconds(1f);

        if (firstChoice.isCorrect && secondChoice.isCorrect)
        {
            StartDialogue(new string[]
            {
                "Bit: Well done! Binary and Assembly are indeed the languages of the machine.",
                "Player: That wasn’t so bad!",
                "Bit: Lets move along!"
            });
        }
        else
        {
            firstChoice.SetGlow(false);
            secondChoice.SetGlow(false);

            StartDialogue(new string[]
            {
                "Bit: Not quite. Remember, one speaks in pure 0s and 1s and the other in mnemonic commands."
            });
        }

        firstChoice = null;
        secondChoice = null;
    }
}
