using System;
using System.Collections;
using System.Collections.Generic;
using __FurtleAll._FurtleScripts.Controllers;
using _Scripts.Runtime.Enums;
using Cinemachine;
using DG.Tweening;
using FurtleGame.UISystem;
using FurtleGame.UpgradeSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class DomeManager : MonoBehaviour
{
    [Header("Public Variables")] public UpgradeButton domeUpgrade;
    public GameObject Dome;
    public Material surfaceMaterial;
    public int domeRadius;

    [Header("Serialized Variables")] [SerializeField]
    private int scaleAmount;

    [SerializeField] private int domeCamDistanceAmount;
    [SerializeField] private ParticleSystem zoneIncreaseParticle;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera domeCamera;
    [SerializeField] private GameObject upgradeDomeCanvas;
    [SerializeField] private Ease doomScaleEase;
    [SerializeField] private Ease doomCamMoveEase;
    [SerializeField] private CutSceneManager cutSceneManager;


    private void Start()
    {
        UpdateDomeRadius();
    }

    private void UpdateDomeRadius()
    {
        domeRadius = (int)Dome.transform.localScale.x;
    }

    private void SetMaterialTile()
    {
        surfaceMaterial.DOTiling(
            new Vector2(surfaceMaterial.mainTextureScale.x + 100, surfaceMaterial.mainTextureScale.y + 100), 0.1f);
    }

    [Button]
    public void IncreaseDomeSize()
    {
        var p = domeUpgrade.price.Evaluate(domeUpgrade.upgrade.level - 1);

        if (SaveData.Ghost < p)
            return;

        if (domeUpgrade.upgrade.level == domeUpgrade.upgrade.maxLevel)
        {
            cutSceneManager.EndAnim();
            return;
        }

        Sequence sequence = DOTween.Sequence();
        var newScale = Dome.transform.localScale + new Vector3(scaleAmount, 0, scaleAmount);

        sequence.AppendCallback(() => Player.Instance.playerMovementController.locked = true);
        sequence.AppendCallback(() => upgradeDomeCanvas.SetActive(false));
        sequence.AppendCallback(() => mainCamera.Priority = 1);
        sequence.AppendInterval(1f);
        sequence.Append(domeCamera.transform
            .DOLocalMove(
                domeCamera.transform.position +
                new Vector3(domeCamDistanceAmount, domeCamDistanceAmount, -domeCamDistanceAmount), 2f)
            .SetEase(doomCamMoveEase));
        sequence.Join(Dome.transform.DOScale(newScale, 1.5f).SetEase(doomScaleEase)
            .OnComplete(() => SetMaterialTile()));
        sequence.Join(DOVirtual.DelayedCall(0f, () => zoneIncreaseParticle.Play()));
        sequence.Join(DOVirtual.DelayedCall(0f, () => SoundManager.Instance.PlaySound(GameSoundType.WinkParticle)));
        sequence.AppendCallback(() => MiniTaskSystem.Instance.IncreaseStep("UpgradeDome"));
        sequence.Join(DOVirtual.DelayedCall(0.1f, () => zoneIncreaseParticle.Play()));
        sequence.Join(DOVirtual.DelayedCall(0.1f, () => SoundManager.Instance.PlaySound(GameSoundType.ShineParticle)));
        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(() => UpdateDomeRadius());
        sequence.AppendCallback(() => mainCamera.Priority = 11);
        sequence.AppendCallback(() => upgradeDomeCanvas.SetActive(true));
        sequence.AppendCallback(() => Player.Instance.playerMovementController.locked = false);
    }


    private void OnApplicationQuit()
    {
        ResetMaterialTile();
    }

    private void OnDisable()
    {
        ResetMaterialTile();
    }

    private void ResetMaterialTile()
    {
        Vector2 originalTileValue = new Vector2(300, 300);
        surfaceMaterial.mainTextureScale = originalTileValue;
    }
}