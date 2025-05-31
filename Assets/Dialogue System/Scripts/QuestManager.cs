// DIALOGUE SYSTEM made by James Shipp
// Last updated 9/28/23

using UnityEngine;
using System;

/*
 * This script should go on an empty gameobject as manager
 * that only needs one instance in the whole game (set up 
 * as singleton). It is referenced to check and update quest
 * statuses as loaded from the quest list CSV file
 */
public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset questListCSV;
    [SerializeField, Tooltip("This list will be loaded in at runtime, don't add anything here!")]
    private Quest[] questList;

    // singleton
    private static QuestManager instance;

    private void Awake()
    {
        // setting up singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    public static QuestManager Instance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (questListCSV != null)
            questList = CSVReader.ReadQuestListCSV(questListCSV);
        else
            throw new Exception("CSV file not configured for QuestManager quest list");
    }

    // check if a quest has been completed or not
    public bool GetQuestCompletion(string questID)
    {
        Quest q = Array.Find(questList, x => string.Compare(x.GetID(), questID) == 0);
        if (q == null)
            throw new Exception("Searched quest list for non-existant quest! Did you make a typo somewhere?\n");
        return q.GetCompletion();
    }

    // update a quest's completion status
    public void SetQuestCompletion(string questID, bool completionStatus)
    {
        Quest q = Array.Find(questList, x => x.GetID() == questID);
        if (q == null)
            throw new Exception("Searched quest list for non-existant quest! Did you make a typo somewhere?\n");
        q.SetCompletion(completionStatus);
    }

    // search through the quests to see if there is one with this name
    // (used for checking repeat requirements, whether it's a quest or
    // or item or convo ID)
    public bool CheckIfTagIsQuest(string tag)
    {
        Quest q = Array.Find(questList, x => x.GetID() == tag);
        if (q == null)
            return false;
        return true;
    }
}

// class to store quest data loaded from quest list CSV file
[System.Serializable]
public class Quest
{
    [SerializeField]
    private string id;
    [SerializeField]
    private bool complete;

    public string GetID() => id;
    public bool GetCompletion() => complete;

    public Quest(string questId)
    {
        id = questId;
        complete = false;
    }

    public void SetCompletion(bool completionStatus)
    {
        complete = completionStatus;
    }
}
