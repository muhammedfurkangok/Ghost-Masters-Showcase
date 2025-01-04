using System;
using System.Collections.Generic;
using FurtleGame.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class QuestManager : SingletonMonoBehaviour<QuestManager>
{
    public List<QuestSO> activeQuests = new List<QuestSO>();
    public GameObject QuestDisplayGameObject;
    public QuestDisplay questDisplay;

    private void Start()
    {
        if (activeQuests.Count < 1)
        {
            QuestDisplayGameObject.SetActive(false);
        }
    }

    [Button]
    public void AddQuest(QuestSO newQuest)
    {
        
        if (!activeQuests.Contains(newQuest))
        {
            activeQuests.Add(newQuest);
            questDisplay.AddItem(newQuest);
            Debug.Log("Yeni görev eklendi: " + newQuest.questName);
        }
        
        if(activeQuests.Count >0)
            QuestDisplayGameObject.SetActive(true);
    }

    public void UpdateQuestProgress(ResourceSO catchableResourceSO)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questResources.Length > 0)
            {
                foreach (var resource in quest.questResources)
                {
                    if (resource.id == catchableResourceSO.id && !quest.canBeGivenObject)
                    {
                        quest.UpdateProgress(catchableResourceSO);
                        questDisplay.UpdateQuestsProgresssions();
                    }
                }
            }
        }
    }

    public void CompleteQuest(QuestSO quest)
    {
        if (activeQuests.Contains(quest) && quest.isCompleted)
        {
            activeQuests.Remove(quest);
            quest.QuestCompleted();
            questDisplay.RemoveItem(quest);
            Debug.Log("Görev tamamlandı: " + quest.questName);
        }
        if (activeQuests.Count < 1)
        {
            QuestDisplayGameObject.SetActive(false);
        }
    }
}