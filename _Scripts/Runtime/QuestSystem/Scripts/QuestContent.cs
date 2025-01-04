using TMPro;
using UnityEngine;

public class QuestContent : MonoBehaviour
{
    public QuestSO quest;
    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questProgress;
    
    public void SetQuest(QuestSO questSo)
    {
        quest = questSo;
        SetQuestContent();
    }
    public void SetQuestContent()
    {
        questTitle.text = quest.questName;
        questProgress.text = quest.progress + "/" + quest.goal;
    }

}
