using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "Furtle Game/Quest System/QuestSO", order = 0)]
public class QuestSO : ScriptableObject
{
    public string questName;
    public string questDescription;
    public int progress;
    public int goal;
    public bool isCompleted;
    public bool canBeGivenObject;
    public Sprite questIcon;
    public ResourceSO[] questResources;
    public RewardSO[] rewards;

    public void UpdateProgress(ResourceSO resource)
    {
        if (resource.id == questResources[0].id && !isCompleted)
        {
            progress++;
            if (progress >= goal)
            {
                isCompleted = true;
            }
        }
    }

    public void QuestCompleted()
    {
        foreach (var reward in rewards)
        {
            reward.RewardPlayer();
        }
    }

    public void ResetQuest()
    {
        isCompleted = false;
        progress = 0;
    }
}