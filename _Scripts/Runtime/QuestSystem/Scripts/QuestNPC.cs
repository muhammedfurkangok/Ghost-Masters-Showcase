using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class QuestNPC : MonoBehaviour
{
    public Button completeButton;
    public QuestManager questManager;
    public QuestSO questToOffer;
    public TextMeshProUGUI questProgressionText;
    public TextMeshProUGUI questProgressionText2;
    public Image questFiller;

    public GameObject questionMark;
    public GameObject exclamationMark;
    public GameObject TriggerVisual;
    public GameObject InteractVisual;
   
    public bool isQuestTaken;
    
    

    private void Start()
    {
        TriggerVisual.SetActive(false);
        questionMark.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerVisual.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerVisual.SetActive(false);
        }
    }

    public void Interact()
    {
        if (!isQuestTaken)
        {
            questManager.AddQuest(questToOffer);
            isQuestTaken = true;
            questionMark.SetActive(false);
            exclamationMark.SetActive(true);
        }

        InteractVisual.SetActive(true);
        CheckQuestComplete();
    }

    public void CheckQuestComplete()
    {
        if (!questToOffer.isCompleted)
        {
            completeButton.interactable = false;
            questProgressionText.text = questToOffer.progress + "/" + questToOffer.goal;
            questProgressionText2.text = questToOffer.progress + "/" + questToOffer.goal;
            questFiller.fillAmount = (float)questToOffer.progress / questToOffer.goal;
        }
        else
        {
            completeButton.interactable = true;
            questProgressionText.text = "Completed";
            questProgressionText2.text = questToOffer.progress + "/" + questToOffer.goal;
            questFiller.fillAmount = 1;
        }
    }

    public void CompleteQuest()
    {
        questManager.CompleteQuest(questToOffer);
        InteractVisual.SetActive(false);
        MiniTaskSystem.Instance.IncreaseStep("CompleteAQuest");
        DOVirtual.DelayedCall( 1.5f, () => DestroyQuestNPC());
       
    }

    private void DestroyQuestNPC()
    {
        transform.DOScale(Vector3.zero, 0.5f);
    }
}