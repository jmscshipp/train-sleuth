using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLook : MonoBehaviour
{
    [SerializeField]
    private Transform NPCHead;

    [SerializeField]
    private Vector3 worldUP;
    [SerializeField]
    private Vector3 adjustment;

    private Quaternion initialRot;
    private Quaternion finalRot;
    private Quaternion savedFinalRot;
    private bool useLookRot = false;
    private bool lerpingThere = false;
    private bool lerpingBack = false;
    private float lerpCounter = 0f;

    // used with player interaction 
    private bool looking;
    private NPCDialogueManager dialogueManager;
    private Interactable interactable;

    // Start is called before the first frame update
    void Start()
    {
        initialRot = NPCHead.rotation;
        dialogueManager = transform.parent.GetComponent<NPCDialogueManager>();
        interactable = transform.parent.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookRot = Quaternion.LookRotation(Camera.main.transform.position -
            NPCHead.transform.position, worldUP).eulerAngles;
        finalRot = Quaternion.Euler(lookRot + adjustment);

        if (lerpingThere)
        {
            lerpCounter += Time.deltaTime * 3f;
            NPCHead.rotation = Quaternion.Lerp(initialRot, finalRot, lerpCounter);
            if (lerpCounter >= 1f)
            {
                useLookRot = true;
                lerpingThere = false;
            }
        }
        else if (lerpingBack)
        {
            lerpCounter += Time.deltaTime * 3f;
            NPCHead.rotation = Quaternion.Lerp(savedFinalRot, initialRot, lerpCounter);
            if (lerpCounter >= 1f)
            {
                useLookRot = false;
                lerpingBack = false;
            }
        }
        else if (useLookRot)
        {
            NPCHead.rotation = finalRot;
        }
        else
            NPCHead.rotation = initialRot;

    }

    public void HoverOver()
    {
        if (looking)
        {
            DialogueDisplay.Instance().ActivateTalkPopup();
        }
    }

    public void Interact()
    {
        if (looking)
        {
            dialogueManager.StartConversation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            lerpingBack = false;
            lerpCounter = 0f;
            lerpingThere = true;
            looking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            lerpingThere = false;
            lerpCounter = 0f;
            savedFinalRot = finalRot;
            lerpingBack = true;
            looking = false;
        }
    }
}
