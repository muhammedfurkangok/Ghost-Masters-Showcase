using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    public QuestManager questManager;
    public TextMeshProUGUI questCount;
    public GameObject questContentPrefab;
    [SerializeField] private Button questButton;
    [SerializeField] private RectTransform questContent;
    [SerializeField] private RectTransform arrowImage;
    [SerializeField] private float expandedHeight = 400f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private List<RectTransform> questItems;
    private bool isExpanded = false;

    private void Start()
    {
        UpdateQuestCount();
       
        questButton.onClick.AddListener(ToggleQuestList);
        foreach (var item in questItems)
        {
            item.localScale = Vector3.zero; 
        }

        questContent.sizeDelta = new Vector2(questContent.sizeDelta.x, 0); 
    }
 
    public void AddItem(QuestSO quest)
    {
        if (isExpanded)
        {
            ToggleQuestList();
        }
        
        var newQuestItem = Instantiate(questContentPrefab, questContent);
        newQuestItem.transform.localScale = Vector3.zero;
        newQuestItem.GetComponent<QuestContent>().SetQuest(quest);
        questItems.Add(newQuestItem.GetComponent<RectTransform>());
        UpdateQuestCount();
    }
    
    public void RemoveItem(QuestSO quest)
    {
        var questItem = questItems.Find(x => x.GetComponent<QuestContent>().quest == quest);
        questItems.Remove(questItem);
        Destroy(questItem.gameObject);
        UpdateQuestCount();
    }

    [Button]
    private void ToggleQuestList()
    {
        if (isExpanded)
        {
            foreach (var item in questItems)
            {
                item.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InOutQuad);
            }
            questContent.DOSizeDelta(new Vector2(questContent.sizeDelta.x, 0), animationDuration).SetEase(Ease.InOutQuad);
            arrowImage.DORotate(Vector3.zero, animationDuration).SetEase(Ease.OutBack);
        }
        else
        {
            questContent.DOSizeDelta(new Vector2(questContent.sizeDelta.x, expandedHeight), animationDuration)
                .SetEase(Ease.InOutQuad);

            StartCoroutine(ExpandItems());

            arrowImage.DORotate(new Vector3(0, 0, 180), animationDuration)
                .SetEase(Ease.OutBack); 
        }

        isExpanded = !isExpanded; 
    }

    public void UpdateQuestsProgresssions()
    {
        foreach (var item in questItems)
        {
            item.GetComponent<QuestContent>().SetQuestContent();
        }
    }

    public void UpdateQuestCount()
    {
        questCount.text = $"Quests({questManager.activeQuests.Count})";
    }

    private IEnumerator ExpandItems()
    {
        foreach (var item in questItems)
        {
            item.DOScale(new Vector3(4.2f, 1.2f, 0.8f), animationDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f); 
        }
    }
}