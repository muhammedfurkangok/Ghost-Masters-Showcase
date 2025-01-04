using System;
using __FurtleAll._FurtleScripts.Controllers;
using DG.Tweening;
using FurtleGame.EventSystem;
using FurtleGame.Singleton;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Runtime.Managers
{
    public class ResourceManager : SingletonMonoBehaviour<ResourceManager>
    {
        [Header("Backpack")] 
        public Backpack backpack;

        [Header("Resources")]
        public GhostDisplay ghostDisplay;
        public GearDisplay gearDisplay;

        [Header("Resources Texts")]
        public TextMeshProUGUI capacityText;
        public Image capacityFillArea;
        public GameObject notEnoughSpacePanel;
        
        [SerializeField] private ParticleSystem ghostParticle;
        [SerializeField] private GameObject inventoryFullPanel;
        [SerializeField] private GameObject capacityPanel;


        private void Start()
        {
            backpack = Player.Instance.backpack;
            ghostDisplay = FindObjectOfType<GhostDisplay>();
            gearDisplay = FindObjectOfType<GearDisplay>();

            EventManager.StartListening("OnInventoryChanged", UpdateInventoryTexts);
            EventManager.StartListening("OnResourcesChanged", UpdateResourceTexts);
            EventManager.StartListening("OnInventoryFull", OnInventoryFull);
            EventManager.StartListening("OnInventoryCleared", OnInventoryCleared);
            UpdateInventoryTexts();
        }

        private void OnInventoryCleared()
        {
            inventoryFullPanel.SetActive(false);
            capacityPanel.SetActive(true);
        }

        private void OnInventoryFull()
        {
            inventoryFullPanel.SetActive(true);
            capacityPanel.SetActive(false);
        }

        public void UpdateResourceTexts()
        {
            UpdateGhostResource();
            UpdateGearResource();
        }

        public void UpdateInventoryTexts()
        {
            UpdateCapacityText();
        }

        private void UpdateGearResource()
        {
            gearDisplay.valueField.text = gearDisplay.GetValue().ToString();
        }

        public void UpdateGhostResource()
        {
            ghostDisplay.valueField.text = ghostDisplay.GetValue().ToString();
        }

        [Button]
        public void UpdateCapacityText()
        {
            capacityText.text = $"CAPACITY ({backpack.GetTotalAmount()}/{backpack.capacity})";
            capacityFillArea.DOFillAmount(backpack.GetTotalAmount() / (float)backpack.capacity, .2f);
            backpack.UpdateLiquidVolume(backpack.GetTotalAmount());
        }


        private void OnDisable()
        {
            EventManager.StopListening("OnInventoryChanged", UpdateInventoryTexts);
            EventManager.StopListening("OnResourcesChanged", UpdateResourceTexts);
            EventManager.StopListening("OnInventoryFull", OnInventoryFull);
            EventManager.StopListening("OnInventoryCleared", OnInventoryCleared);
        }
    }
}