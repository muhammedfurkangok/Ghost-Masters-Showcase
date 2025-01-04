// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using FurtleGame.EventSystem;
// using FurtleGame.UpgradeSystem;
// using Lofelt.NiceVibrations;
// using System;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UnityEngine.Events;
// using System.Collections.Generic;
// using FurtleGame;
//
// public class UpgradeButtonResources : MonoBehaviour
// {
//     [Header("Properties")]
//     public Upgrade upgrade;
//     public UpgradeResourcePrice resourcePrice;  // Yeni sistem ile çalışacak
//     public UpgradeCapacity capacity;
//     public bool isAd = false;
//     public bool ForceDeactive { get => forceDeactive; set => forceDeactive = value; }
//
//     [Header("UI Elements")]
//     public Shadow shadow;
//
//     public AudioSource audioSource;
//     public AudioClip moneySound;
//
//     public List<UpgradeRequirement> requirements = new List<UpgradeRequirement>();
//     public List<ColoredPart> coloredParts = new List<ColoredPart>();
//     public List<Color> shadowColors = new List<Color>();
//     public List<StateToGameObject> stateToGameObjects = new List<StateToGameObject>();
//
//     [Header("Events")]
//     public UnityEvent OnClick;
//
//     private ButtonParser parser;
//     private Button button;
//     private bool forceDeactive = false;
//
//     private void Awake()
//     {
//         parser = GetComponent<ButtonParser>();
//         button = GetComponent<Button>();
//
//         if (parser == null)
//             parser = gameObject.AddComponent<ButtonParser>();
//     }
//
//     private void Start()
//     {
//         if (upgrade == null)
//             return;
//
//         upgrade = UpgradeManager.Instance.Initialize(upgrade);
//
//         UpdateButtons();
//
//         if (capacity != null)
//             capacity.OnChange.AddListener(OnCapacityChanged);
//     }
//
//     private void OnEnable()
//     {
//         EventManager.StartListening("OnGhostChanged", (UnityAction<float>)OnMoneyChanged);
//         OnMoneyChanged((int)SaveData.Ghost);
//     }
//
//     private void OnDisable()
//     {
//         EventManager.StopListening("OnGhostChanged", (UnityAction<float>)OnMoneyChanged);
//     }
//
//     void OnCapacityChanged(UpgradeCapacity capacity)
//     {
//         if (upgrade == null)
//             return;
//
//         UpdateButtons();
//     }
//
//     private void OnMoneyChanged(float money)
//     {
//         if (upgrade == null)
//             return;
//
//         UpdateButtons();
//     }
//
//     public void UpdateButtons()
//     {
//         if (!upgrade.infinite && upgrade.IsMaxed() || (capacity != null && capacity.IsMaxed()))
//         {
//             button.interactable = false;
//             foreach (var coloredPart in coloredParts)
//                 coloredPart.SetActive(false, this);
//
//             ChangeState("Disabled");
//         }
//         else if (forceDeactive)
//         {
//             ChangeState("Disabled");
//
//             foreach (var coloredPart in coloredParts)
//                 coloredPart.SetActive(false, this);
//         }
//         else if (!HasSufficientResources(upgrade.level - 1))
//         {
//             foreach (var coloredPart in coloredParts)
//                 coloredPart.SetActive(false, this);
//
//             ChangeState("Disabled");
//         }
//         else if (!requirements.TrueForAll(r => r.Evaluate(upgrade.level)))
//         {
//             button.interactable = false;
//             foreach (var coloredPart in coloredParts)
//                 coloredPart.SetActive(false, this);
//
//             ChangeState("Disabled");
//         }
//         else
//         {
//             button.interactable = true;
//             foreach (var coloredPart in coloredParts)
//                 coloredPart.SetActive(true, this);
//
//             ChangeState("Active");
//         }
//
//         parser.SetText("level", "Level " + upgrade.level);
//
//         if (!upgrade.infinite && upgrade.IsMaxed()) parser.SetFullText("price", "FULL");
//         else parser.SetText("price", GetResourcePrices(upgrade.level - 1));
//     }
//
//     private bool HasSufficientResources(int level)
//     {
//         if (resourcePrice == null || resourcePrice.resources == null || resourcePrice.resources.Length <= level)
//             return false;
//         
//         foreach (var resourcePair in resourcePrice.resources[level])
//         {
//             var currentAmount = SaveData.GetResourceAmount(resourcePair.Key);
//             if (currentAmount < resourcePair.Value)
//             {
//                 button.interactable = false;
//                 return false; 
//             }
//         }
//         button.interactable = true;
//         return true;
//     }
//     private string GetResourcePrices(int level)
//     {
//         if (resourcePrice == null || resourcePrice.resources == null || resourcePrice.resources.Length <= level)
//             return "";
//
//         string priceString = "";
//         foreach (var resourcePair in resourcePrice.resources[level])
//         {
//             priceString += $"{resourcePair.Value} ";
//         }
//
//         return priceString.Trim();
//     }
//
//     public void Click()
//     {
//         if (isAd)
//         {
//             // OnClickAdStrategy();
//         }
//         else
//         {
//             OnClickMoneyStrategy();
//         }
//     }
//
//     public void OnClickMoneyStrategy()
//     {
//         var level = upgrade.level - 1;
//
//         if (!HasSufficientResources(level))
//             return;
//
//         button.enabled = false;
//
//         DOTween.Sequence()
//             .Append(transform.DOPunchScale(Vector3.one * .1f, .25f))
//             .AppendCallback(() =>
//             {
//                 button.enabled = true;
//             });
//
//         audioSource.PlayOneShot(moneySound);
//         DeductResources(level);
//
//         var succeed = UpgradeManager.Instance.Upgrade(upgrade);
//
//         if (succeed)
//         {
//             UpdateButtons();
//             EventManager.TriggerEvent("OnUpgrade", upgrade);
//             EventManager.TriggerEvent("OnAfterUpgrade", upgrade);
//             HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
//             OnClick.Invoke();
//         }
//     }
//
//     private void DeductResources(int level)
//     {
//         foreach (var resourcePair in resourcePrice.resources[level])
//         {
//             SaveData.ModifyResource(resourcePair.Key, -resourcePair.Value);
//         }
//     }
//
//     public void ChangeState(string state)
//     {
//         foreach (var stateToGameObject in stateToGameObjects)
//         {
//             stateToGameObject.gameObject.SetActive(stateToGameObject.state == state);
//         }
//
//         shadow.effectColor = shadowColors[state == "Active" ? 1 : 0];
//     }
// }
