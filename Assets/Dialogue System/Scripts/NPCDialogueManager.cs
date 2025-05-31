// DIALOGUE SYSTEM made by James Shipp
// Last updated 9/28/23

using UnityEngine;
using System;

/*
 * This script goes on an NPC and manages the dialogue this NPC
 * presents to the player, loaded from a CSV file. Call 
 * StartConversation() from an outside source to bring up
 * dialogue with this NPC- which of the configured conversations
 * is selected is determined by what conversation requirements
 * have been met, which convos have already been read, etc.
 */
public class NPCDialogueManager : MonoBehaviour
{
    [SerializeField]
    private string characterName;
    [SerializeField]
    private TextAsset dialogueCSV;
    [SerializeField]
    private float talkSpeed = 70f;
    [SerializeField]
    private string voiceSoundName = "DialogueSoundNormal";
    [SerializeField, Tooltip("This list will be loaded in at runtime, don't add anything here!")]
    private DialogueConversation[] conversations;
    DialogueConversation currentConvo;

    // Start is called before the first frame update
    void Start()
    {
        currentConvo = null;

        // load conversations from CSV file
        if (dialogueCSV != null)
        {
            conversations = CSVReader.ReadNpcCSV(dialogueCSV);

            foreach (DialogueConversation convo in conversations)
            {
                convo.SetCharacterName(characterName);
                convo.SetTalkSpeed(talkSpeed);
                convo.SetVoiceSoundName(voiceSoundName);
            }
        }
        else
            throw new Exception("CSV file not configured for " + characterName);
    }

    // begin conversation with this NPC. which convo is selected is based off of
    // what requirements are met, what conversations have already been read, etc
    public void StartConversation()
    {
        // defaults to final entry
        currentConvo = conversations[conversations.Length - 1];

        // decide what conversation to start
        for (int i = 0; i < conversations.Length; i++)
        {
            // if the convo requirements are met AND either the convo hasn't been read or the repeat requirements haven't been met
            if (QuestRequirementsMet(conversations[i].GetQuestRequirements()) && ItemRequirementsMet(conversations[i].GetItemRequirements()) && 
                (!conversations[i].GetReadStatus() || !RepeatRequirementsMet(conversations[i].GetRepeatRequirements())))
            {
                currentConvo = conversations[i];
                break;
            }
        }

        // send to dialogue display
        DialogueDisplay.Instance().OnDialogueFinish.AddListener(ConversationCompleted);
        DialogueDisplay.Instance().ActivateDisplay(currentConvo);

        // look at this ^^
    }

    // triggered by dialogue display event
    private void ConversationCompleted()
    {
        // update quest list with triggers
        string[] triggers = currentConvo.GetCompletionTriggers();
        for (int i = 0; i < triggers.Length; i++)
        {
            DialogueConversation triggerConvo = Array.Find(conversations, x => x.GetConversationID() == triggers[i]);
            if (triggerConvo != null)
                triggerConvo.SetReadStatus(true); // trigger is for marking a convo as read
            else
            {
                if (QuestManager.Instance().CheckIfTagIsQuest(triggers[i]))
                    QuestManager.Instance().SetQuestCompletion(triggers[i], true); // trigger is for marking a quest as complete
                else
                    ItemManager.Instance().SetItemHolding(triggers[i]); // trigger is for giving or taking an item
            }
        }

        currentConvo.SetReadStatus(true);
        DialogueDisplay.Instance().OnDialogueFinish.RemoveListener(ConversationCompleted);
    }

    private bool QuestRequirementsMet(string[] requirements)
    {
        for (int i = 0; i < requirements.Length; i++)
        {
            if (!QuestManager.Instance().GetQuestCompletion(requirements[i]))
                return false;
        }

        return true;
    }

    private bool RepeatRequirementsMet(string[] requirements)
    {
        for (int i = 0; i < requirements.Length; i++)
        {
            if (QuestManager.Instance().CheckIfTagIsQuest(requirements[i])) // quest requirement
            {
                if (!QuestManager.Instance().GetQuestCompletion(requirements[i])) // not complete
                    return false;
            }
            else // item requirement
            {
                if (!ItemManager.Instance().GetItemHolding(requirements[i])) // don't have item
                    return false;
            }
        }

        return true;
    }

    private bool ItemRequirementsMet(string[] requirements)
    {
        for (int i = 0; i < requirements.Length; i++)
        {
            if (!ItemManager.Instance().GetItemHolding(requirements[i]))
                return false;
        }

        return true;
    }
}

// class to store info from each conversation that's loaded from CSV file.
// keeps track of convo requirements, read status, etc
[System.Serializable]
public class DialogueConversation
{
    // data that's specific to each conversation
    [SerializeField]
    private string conversationId;
    [SerializeField]
    private bool alreadyRead;
    [SerializeField]
    private string[] questRequirements;
    [SerializeField]
    private string[] itemRequirements;
    [SerializeField]
    private string[] repeatRequirements;
    [SerializeField]
    private string[] completionTriggers;
    [SerializeField]
    private string[] dialogueLines;
    [SerializeField]

    // data that's the same across all of an NPC's convos
    private string characterName;
    private float talkSpeed;
    private string voiceSoundName;

    public void SetConversationID(string newID)
    {
        conversationId = newID;
    }

    public string GetConversationID() => conversationId;

    public void SetReadStatus(bool readStatus)
    {
        alreadyRead = readStatus;
    }

    public bool GetReadStatus() => alreadyRead;

    public void SetQuestRequirements(string[] requirements)
    {
        questRequirements = requirements;
    }

    public string[] GetQuestRequirements() => questRequirements;

    public void SetItemRequirements(string[] requirements)
    {
        itemRequirements = requirements;
    }

    public string[] GetItemRequirements() => itemRequirements;

    public void SetRepeatRequirements(string[] requirements)
    {
        repeatRequirements = requirements;
    }

    public string[] GetRepeatRequirements() => repeatRequirements;

    public void SetCompletionTriggers(string[] requirements)
    {
        completionTriggers = requirements;
    }

    public string[] GetCompletionTriggers() => completionTriggers;

    public void SetDialogueLines(string[] dialogue)
    {
        dialogueLines = dialogue;
    }

    public string[] GetDialogueLines() => dialogueLines;

    public void SetTalkSpeed(float newTalkSpeed)
    {
        talkSpeed = newTalkSpeed;
    }

    public float GetTalkSpeed() => talkSpeed;

    public void SetVoiceSoundName(string newVoiceSoundName)
    {
        voiceSoundName = newVoiceSoundName;
    }

    public string GetVoiceSoundName() => voiceSoundName;

    public void SetCharacterName(string charName)
    {
        characterName = charName;
    }

    public string GetCharacterName() => characterName;
}