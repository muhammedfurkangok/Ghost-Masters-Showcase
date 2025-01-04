using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCanvas : MonoBehaviour
{
    [SerializeField] private Image getIcon;
    [SerializeField] private TextMeshProUGUI getValue;
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questProgression;
    [SerializeField] private QuestSO questResources;

    public void Init()
    {
        if (questResources == null) return; 

        if (questResources.questResources.Length > 0 && questResources.questResources[0] != null)
        {
            getIcon.sprite = questResources.questResources[0].icon;
        }

        questTitle.text = questResources.questName;
        questDescriptionText.text = questResources.questDescription;
        questProgression.text = questResources.progress + "/" + questResources.goal;

        if (questResources.rewards.Length > 0)
        {
            getValue.text = questResources.rewards[0].count.ToString();
        }
    }

    private void OnValidate()
    {
        Init(); 
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}