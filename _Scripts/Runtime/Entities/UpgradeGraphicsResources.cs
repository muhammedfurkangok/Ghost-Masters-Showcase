// using System.Collections;
// using System.Collections.Generic;
// using FurtleGame.EventSystem;
// using FurtleGame.UpgradeSystem;
// using TMPro;
// using UnityEngine;
//
// public class UpgradeGraphicsResources : MonoBehaviour
// {
//     public UpgradeButtonResources upgradeButton;
//     public TMP_Text levelDisplay;
//     public List<LevelBar> levelBars = new List<LevelBar>();
//
//     void Start()
//     {
//         RenderBars();
//     }
//
//     private void OnEnable() {
//         EventManager.StartListening("OnUpgrade", OnUpgrade);
//     }
//
//     private void OnDisable() {
//         EventManager.StopListening("OnUpgrade", OnUpgrade);
//     }
//
//     private void OnUpgrade(Upgrade upgrade)
//     {
//         RenderBars();
//     }
//
//     public void ResetAllColor()
//     {
//         foreach (var item in levelBars)
//         {
//             item.ResetColor();
//         }
//     }
//
//     public void RenderBars()
//     {
//         for (int i = 0; i < levelBars.Count; i++)
//         {
//             if (i <= Mathf.Abs((upgradeButton.upgrade.level - 1) % levelBars.Count))
//                 levelBars[i].ChangeColor();
//             else
//                 levelBars[i].ResetColor();
//         }
//
//         if (levelDisplay) levelDisplay.text = "Level " + upgradeButton.upgrade.level;
//     }
// }