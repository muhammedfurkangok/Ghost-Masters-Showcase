using System.Collections;
using System.Collections.Generic;
using __FurtleAll._FurtleScripts.Controllers;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeButton : MonoBehaviour
{
    [Header("Exchange Settings")]
    public string requiredItem;  
    public int requiredAmount = 1; 
    public string rewardItem;  
    public int rewardAmount = 100;  

    [Header("UI Elements")]
    public Button exchangeButton;
    
    [Header("Refs")]
    public GhostDisplay ghostDisplay;


    public void Interact()
    {
        if (exchangeButton.interactable && SaveData.Gear >= requiredAmount)
        {
           MiniTaskSystem.Instance.IncreaseStep("MakeExchange");
            SaveData.Gear -= requiredAmount;

            MoneyPopper.Spawn("Ghost", new PopSettings
            {
                count = rewardAmount / 10,
                value = rewardAmount, 
                source = Player.Instance.transform.position,
                bringForward = false
            });

            Debug.Log($"Exchanged {requiredAmount} {requiredItem} for {rewardAmount} {rewardItem}.");
            
            UpdateButtonInteractable();
        }
        else
        {
            Debug.Log("Not enough resources for exchange.");
        }
    }

    public void UpdateButtonInteractable()
    {
        if (SaveData.Gear >= requiredAmount)
        {
            exchangeButton.interactable = true;
        }
        else
        {
            exchangeButton.interactable = false;
        }
    }
}