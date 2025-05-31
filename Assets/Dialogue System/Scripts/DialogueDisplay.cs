// DIALOGUE SYSTEM made by James Shipp
// Last updated 9/28/23

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System;

/*
 * This script should go on an instance of the DialogueCanvas
 * prefab. It controls the display of text from NPCDialogueManagers
 * and facilitates the opening and closing of the dialogue box
 * and associated side effects! 
 */
public class DialogueDisplay : MonoBehaviour
{
    // references set up in the inspector
    [SerializeField]
    private TMP_Text dialogueText;
    [SerializeField]
    private TMP_Text nameText;
    [SerializeField, Tooltip("Length of delay between dialogue box open animation starting and text beginning to type")]
    private float openAnimationLength;
    [SerializeField, Tooltip("Length of delay between dialogue box close animation starting canvas being switched off")]
    private float closeAnimationLength;
    [SerializeField, Tooltip("Sound must be configured in AudioManager, use same name here")]
    private string dialogueBoxOpenSound;
    [SerializeField, Tooltip("Sound must be configured in AudioManager, use same name here")]
    private string dialogueBoxCloseSound;

    // text parsing bookkeeping
    private bool active;
    bool typing = false;
    private int lineIndex;
    private DialogueConversation currentConvo;

    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueFinish;

    private static DialogueDisplay instance;

    // text lerp in effect variables
    Mesh mesh;
    Vector3[] vertices;
    int characterCount = 0;
    float xTracker = 0f;
    float shiftDistance = 5f;

    private void Awake()
    {
        // setting up singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    public static DialogueDisplay Instance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        lineIndex = 0;
        currentConvo = null;

        GetComponent<Canvas>().enabled = false;
        dialogueText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (typing)
                {
                    characterCount = dialogueText.textInfo.characterCount - 1;

                    // make sure to play noise when skipping to end of dialogue
                    if ((characterCount + 1) % 2 != 0)
                        AudioManager.Instance().PlaySound(currentConvo.GetVoiceSoundName());
                }
                else
                    ShowNextLine();
            }

            if (!typing)
                return;

            dialogueText.ForceMeshUpdate();
            mesh = dialogueText.mesh;
            vertices = mesh.vertices;

            Color[] colors = mesh.colors;

            // keep already typed characters black
            for (int i = 0; i < characterCount; i++)
            {
                TMP_CharacterInfo completedChar = dialogueText.textInfo.characterInfo[i];

                int completedCharIndex = completedChar.vertexIndex;
                colors[completedCharIndex] = Color.black;
                colors[completedCharIndex + 1] = Color.black;
                colors[completedCharIndex + 2] = Color.black;
                colors[completedCharIndex + 3] = Color.black;
            }

            // lerp in current character
            if (dialogueText.textInfo.characterInfo[characterCount].isVisible)
            {
                xTracker -= Time.deltaTime * currentConvo.GetTalkSpeed();
                float lerpProgress = xTracker / shiftDistance;

                Vector3 offset = new Vector3(Mathf.Lerp(0, shiftDistance, lerpProgress), 0f, 0f);
                int index = dialogueText.textInfo.characterInfo[characterCount].vertexIndex;

                // slide character over
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;

                // lerp transparent -> black
                colors[index] = Color.Lerp(Color.black, Color.clear, lerpProgress);
                colors[index + 1] = Color.Lerp(Color.black, Color.clear, lerpProgress);
                colors[index + 2] = Color.Lerp(Color.black, Color.clear, lerpProgress);
                colors[index + 3] = Color.Lerp(Color.black, Color.clear, lerpProgress);

                if (xTracker <= 0)
                {
                    NextCharacter();
                    if (characterCount % 2 == 0)
                        AudioManager.Instance().PlaySound(currentConvo.GetVoiceSoundName());
                }
            }
            else
                NextCharacter();

            mesh.vertices = vertices;
            mesh.colors = colors;
            dialogueText.canvasRenderer.SetMesh(mesh);
        }
    }

    // called by an NPCDialogueManager to display a conversation.
    // opens the dialogue box and begins displaying text
    public void ActivateDisplay(DialogueConversation convo)
    {
        if (convo == null)
            throw new Exception("Activated dialogue display for nonexistant conversation");

        currentConvo = convo;
        nameText.text = convo.GetCharacterName();
        lineIndex = 0;

        AudioManager.Instance().PlaySound(dialogueBoxOpenSound);
        GetComponent<Canvas>().enabled = true;
        GetComponent<Animator>().SetBool("activate", true);
        OnDialogueStart.Invoke();
        StartCoroutine(BeginText());
    }

    private IEnumerator BeginText()
    {
        yield return new WaitForSeconds(openAnimationLength);
        dialogueText.gameObject.SetActive(true);
        ShowNextLine();
        active = true;
    }

    private void ShowNextLine()
    {
        if (currentConvo.GetDialogueLines().Length - 1 < lineIndex)
        {
            CloseDisplay();
            return;
        }

        typing = true;
        characterCount = 0;
        dialogueText.color = Color.clear;
        dialogueText.text = currentConvo.GetDialogueLines()[lineIndex];
        lineIndex++;
    }

    private void NextCharacter()
    {
        characterCount++;
        if (characterCount >= dialogueText.textInfo.characterCount)
            typing = false;
        else
            xTracker = shiftDistance;
    }

    private void CloseDisplay()
    {
        StartCoroutine(DelayedClose());
        GetComponent<Animator>().SetBool("activate", false);
       
        dialogueText.gameObject.SetActive(false);
        active = false;
        AudioManager.Instance().PlaySound(dialogueBoxCloseSound);
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(closeAnimationLength);
        GetComponent<Canvas>().enabled = false;
        OnDialogueFinish.Invoke();
    }
}
