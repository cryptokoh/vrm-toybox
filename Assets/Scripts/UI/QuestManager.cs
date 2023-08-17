using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

// Reward class to handle different types of rewards in the game
[System.Serializable]
public class Reward
{
    public enum RewardType { Gold, Item, Experience }

    public RewardType rewardType;
    public string item;
    public int quantity;

    // Function to claim a reward
    public void Claim()
    {
        Debug.Log($"Claimed {quantity} {rewardType}. Item: {item}");
    }
}

// QuestAction class to handle different actions required to complete a quest
[System.Serializable]
public class QuestAction
{
    public enum ActionType { FindItem, BeatBoss, TalkToNPC, BuyItem }

    public ActionType actionType;
    public string title;
    public string description;
    public string hint;
    public string target;
    public int quantity;
    public bool isComplete;
    public Transform position;
    public int quantityCompleted;

    // Function to check if a quest action is completed
    public bool CheckActionComplete(string target, int quantity)
    {
        if (this.target == target)
        {
            quantityCompleted += quantity;
            if (quantityCompleted >= this.quantity)
            {
                isComplete = true;
                Debug.Log($"QuestAction {actionType} complete.");
            }
        }
        return isComplete;
    }
}

// Main Quest class to handle all the properties and actions of a quest
[System.Serializable]
public class Quest
{
    public enum QuestType { MainQuest, SideQuest }

    public QuestType type;
    public string name;
    public string title;
    public string description;
    public bool isFailable;
    public bool isSkippable;
    public bool isFailed;
    public string[] prerequisites;
    public Transform position;
    public List<QuestAction> actions;
    public List<Reward> rewards; // Rewards for completing the quest
    public bool isTimedQuest;
    public float duration; // Duration of the quest in seconds
    private float startTime;

    // Property to check if all quest actions are complete
    public bool IsComplete => actions.All(a => a.isComplete);

    // Property to get the progress of the quest
    public float Progress => actions.Count == 0 ? 0 : (float)actions.Count(a => a.isComplete) / actions.Count;

    // Function to start a quest
    // Function to start a quest
    public void StartQuest(MonoBehaviour behaviour)
    {
        if (isTimedQuest)
        {
            StartTimedQuest(behaviour);
        }
    }

    // Function to start a timed quest
    private void StartTimedQuest(MonoBehaviour behaviour)
    {
        behaviour.StartCoroutine(QuestTimer());
    }

    // Function to check if a timed quest has timed out
    public bool IsTimedOut()
    {
        return isTimedQuest && Time.time - startTime > duration;
    }

    // Function to fail a quest
    public void FailQuest(string reason)
    {
        if (isFailable)
        {
            isFailed = true;
            Debug.Log($"Quest {name} failed. Reason: {reason}");
        }
    }

    // Function to skip a quest
    public void SkipQuest()
    {
        if (isSkippable)
        {
            actions.ForEach(a => a.isComplete = true);
            Debug.Log($"Quest {name} skipped.");
        }
    }




    private IEnumerator QuestTimer()
    {
        yield return new WaitForSeconds(duration);
        FailQuest("Time ran out.");
        Debug.Log($"Quest {name} has timed out.");
    }

}

// Act class to group related quests together
[System.Serializable]
public class Act
{
    public string name;
    public List<Quest> quests;
    public List<Reward> rewards;

    // Property to check if all quests in the act are complete
    public bool IsComplete => quests.All(quest => quest.IsComplete);

    // Property to get the progress of the act
    public float Progress => quests.Count == 0 ? 0 : (float)quests.Count(q => q.IsComplete) / quests.Count;
}

// Main QuestManager class to handle all quest and act operations in the game
public class QuestManager : MonoBehaviour
{
    public List<Act> acts = new List<Act>();
    public List<Quest> failedQuests = new List<Quest>();
    public Quest currentQuest;

    public event Action<Quest> OnQuestToggled;

    private void Start()
    {
        // Initialization code here
    }

    private void Update()
    {
        // Check for end of game
        if (acts.All(a => a.IsComplete))
        {
            Debug.Log("All acts complete. The game ends.");
        }

        // Check if the current quest has timed out or failed
        if (currentQuest != null)
        {
            if (currentQuest.IsTimedOut())
            {
                currentQuest.FailQuest("Time ran out.");
                Debug.Log($"Quest {currentQuest.name} has timed out.");
            }
            else if (currentQuest.isFailed)
            {
                Debug.Log($"Quest {currentQuest.name} has failed.");
                currentQuest = null;
            }
        }
    }

    // Property to get the progress of all main quests
    public float MainQuestProgress
    {
        get
        {
            var mainQuests = acts.SelectMany(a => a.quests).Where(q => q.type == Quest.QuestType.MainQuest).ToList();
            return mainQuests.Count == 0 ? 0 : (float)mainQuests.Count(q => q.IsComplete) / mainQuests.Count;
        }
    }

    // Property to get the progress of all side quests
    public float SideQuestProgress
    {
        get
        {
            var sideQuests = acts.SelectMany(a => a.quests).Where(q => q.type == Quest.QuestType.SideQuest).ToList();
            return sideQuests.Count == 0 ? 0 : (float)sideQuests.Count(q => q.IsComplete) / sideQuests.Count;
        }
    }

    // Function to fail the current quest
    public void FailCurrentQuest(string reason)
    {
        if (currentQuest != null)
        {
            currentQuest.FailQuest(reason);
            currentQuest = null;
        }
    }

    // Function to skip the current quest
    public void SkipCurrentQuest()
    {
        if (currentQuest != null)
        {
            currentQuest.SkipQuest();
            currentQuest = null;
        }
    }

    // Function to check if the prerequisites for a quest are met
    public bool ArePrerequisitesMet(Quest quest)
    {
        foreach (var prereq in quest.prerequisites)
        {
            var act = acts.Find(a => a.quests.Exists(q => q.name == prereq));
            if (act == null)
            {
                Debug.Log($"Prerequisite quest {prereq} not found in any act.");
                return false;
            }

            var prereqQuest = act.quests.Find(q => q.name == prereq);
            if (!prereqQuest.IsComplete)
            {
                Debug.Log($"Prerequisite quest {prereq} not complete.");
                return false;
            }
        }

        return true;
    }

    // Function to toggle the status of a quest
    public void ToggleQuest(string actName, string questName)
    {
        var act = acts.Find(a => a.name == actName);
        if (act != null)
        {
            var quest = act.quests.Find(q => q.name == questName);
            if (quest != null)
            {
                quest.actions.ForEach(a => a.isComplete = !a.isComplete);
                Debug.Log($"Toggled quest {questName} to {quest.IsComplete}");
                OnQuestToggled?.Invoke(quest);
            }
                        else
            {
                Debug.Log($"No quest named {questName} found in act {actName}.");
            }
        }
        else
        {
            Debug.Log($"No act named {actName} found.");
        }
    }

    public void StartQuest(string actName, string questName)
    {
        var act = acts.Find(a => a.name == actName);
        if (act != null)
        {
            var quest = act.quests.Find(q => q.name == questName);
            if (quest != null)
            {
                if (ArePrerequisitesMet(quest))
                {
                    quest.StartQuest(this);  // Passing in QuestManager as MonoBehaviour
                    currentQuest = quest;
                    Debug.Log($"Started quest {questName} in act {actName}.");
                }
                else
                {
                    Debug.Log($"Prerequisites for quest {questName} not met.");
                }
            }
            else
            {
                Debug.Log($"No quest named {questName} found in act {actName}.");
            }
        }
        else
        {
            Debug.Log($"No act named {actName} found.");
        }
    }


    // Function to complete an action of the current quest
    public void CompleteAction(string actionTarget, int quantity)
    {
        if (currentQuest != null)
        {
            var action = currentQuest.actions.Find(a => a.target == actionTarget);
            if (action != null)
            {
                if (action.CheckActionComplete(actionTarget, quantity))
                {
                    Debug.Log($"Action {action.actionType} completed for target {actionTarget}.");
                }
            }
            else
            {
                Debug.Log($"No action with target {actionTarget} found in current quest.");
            }
        }
        else
        {
            Debug.Log($"No current quest active.");
        }
    }

    

}
