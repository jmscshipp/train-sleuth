// DIALOGUE SYSTEM made by James Shipp
// Last updated 9/28/23

using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is made up entirely of static functions, so
 * it doesn't need to exist anywhere in the scene! Different
 * CSV files are passed to functions in this script and 
 * processed into data types to be used by the system
 */
public class CSVReader : MonoBehaviour
{
    // this number directly reflects the CSV layout below,
    // if another column is added to the CSV layout this number
    // will need to be updated to match
    private static int npcCSVColumnNum = 6;

    // CSV layout:
    // Quest Requirements | Item Requirements | Converstation ID | Dialogue | Repeat Requirements | Triggers

    // Reads an NPC CSV file and converts it into a list of DialogueConversations for that NPC
    public static DialogueConversation[] ReadNpcCSV(TextAsset input)
    {
        string[] data = input.text.Split(new string[] { ";", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int rowNum = (data.Length / npcCSVColumnNum) - 1;
        
        List<DialogueConversation> conversations = new List<DialogueConversation>();
        
        string currentConvoID = "";
        DialogueConversation currentConvo = null;
        List<string> currentDialogueLines = null;
        
        // scanning each row
        for (int i = 0; i < rowNum; i++)
        {
            string convoID = data[npcCSVColumnNum * (i + 1) + 2].Trim();
            if (convoID != currentConvoID)
            {
                currentConvoID = convoID;

                // add previous convo to complete list
                if (currentConvo != null)
                {
                    currentConvo.SetDialogueLines(currentDialogueLines.ToArray());
                    conversations.Add(currentConvo);
                }

                // set up new convo
                currentConvo = new DialogueConversation();
                currentConvo.SetConversationID(currentConvoID);
                currentDialogueLines = new List<string>();

                // set up quest requirements
                string[] questIds = data[npcCSVColumnNum * (i + 1)].Trim().Split(' ');
                if (questIds[0] != "~")
                    currentConvo.SetQuestRequirements(questIds);
                else
                {
                    // no quest requirements
                    currentConvo.SetQuestRequirements(new string[] { });
                }

                // set up item requirements
                string[] itemIds = data[npcCSVColumnNum * (i + 1) + 1].Trim().Split(' ');
                if (itemIds[0] != "~")
                    currentConvo.SetItemRequirements(itemIds);
                else
                {
                    // no item requirements
                    currentConvo.SetItemRequirements(new string[] { });
                }

                // add dialogue
                currentDialogueLines.Add(data[npcCSVColumnNum * (i + 1) + 3]);

                // set up repeat requirements
                string[] repeatReqIds = data[npcCSVColumnNum * (i + 1) + 4].Trim().Split(' ');
                if (repeatReqIds[0] != "~")
                    currentConvo.SetRepeatRequirements(repeatReqIds);
                else
                {
                    // no repeat requirements
                    currentConvo.SetRepeatRequirements(new string[] { });
                }

                // set up triggers
                string[] triggerIds = data[npcCSVColumnNum * (i + 1) + 5].Trim().Split(' ');

                if (triggerIds[0] != "~")
                    currentConvo.SetCompletionTriggers(triggerIds);
                else
                {
                    // no triggers
                    currentConvo.SetCompletionTriggers(new string[] { });
                }
            }
            else
            {
                // add dialogue to current convo
                currentDialogueLines.Add(data[npcCSVColumnNum * (i + 1) + 3]);
            }
        }

        // add last conversation
        currentConvo.SetDialogueLines(currentDialogueLines.ToArray());
        conversations.Add(currentConvo);

        return conversations.ToArray();
    }

    // Reads quest list from CSV and converts it into a list of Quests
    public static Quest[] ReadQuestListCSV(TextAsset input)
    {
        string[] data = input.text.Split(new string[] { ";", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int tableSize = data.Length - 1;
        Quest[] questList = new Quest[tableSize];

        for (int i = 0; i < tableSize; i++)
            questList[i] = new Quest(data[(i + 1)].Trim());

        return questList;
    }

    // Reads item list from CSV and converts it into a list of Items
    public static Item[] ReadItemListCSV(TextAsset input)
    {
        string[] data = input.text.Split(new string[] { ";", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int tableSize = data.Length - 1;
        Item[] itemList = new Item[tableSize];

        for (int i = 0; i < tableSize; i++)
            itemList[i] = new Item(data[(i + 1)].Trim());

        return itemList;
    }
}