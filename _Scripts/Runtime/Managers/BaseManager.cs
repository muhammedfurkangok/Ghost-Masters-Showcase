using System;
using System.Linq;
using __FurtleAll._FurtleScripts.Controllers;
using _Scripts.Runtime.Enums;
using DG.Tweening;
using FurtleGame.EventSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Backpack backpack;

    [SerializeField] private GameObject baseUpgradeImage;

    [Header("Displays")] [SerializeField] private GhostDisplay ghostDisplay;
    [SerializeField] private GameObject graveDoor;
    [SerializeField] private GameObject rightSideGrave;
    [SerializeField] private GameObject leftSideGrave;
    [SerializeField] private ParticleSystem soulParticle;
    [SerializeField] private ParticleSystem shineParticle;
    [SerializeField] private float graveAnimDuration;
    [SerializeField] private Ease ease;

    private void Start()
    {
        backpack = Player.Instance.backpack;
        ghostDisplay = FindObjectOfType<GhostDisplay>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RevealUpdateUI();
            Debug.Log($"Backpack has {backpack.GetTotalAmount()} item");
            var position = backpack.transform.position;

            if (backpack.HasItem())
            {
                GraveyardOpenAnim();
                foreach (var resource in backpack.resources)
                {
                    MoneyPopper.Spawn(resource.Key.displayName.Replace(" ", ""), new PopSettings
                    {
                        count = resource.Value / resource.Key.capacityValue,
                        // ReSharper disable once PossibleLossOfFraction
                        value = resource.Key.value * (resource.Value / resource.Key.capacityValue),
                        source = position,
                        bringForward = false
                    });
                }

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    backpack.Clear();
                    BackpackRenderer.Instance.ClearAllItems();
                    EventManager.TriggerEvent("OnInventoryCleared");
                    EventManager.TriggerEvent("OnInventoryChanged");
                });

                SoundManager.Instance.PlaySound(GameSoundType.Base);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideUpdateUI();
            GraveyardCloseAnim();
        }
    }

    private void RevealUpdateUI()
    {
        baseUpgradeImage.SetActive(true);
    }

    private void HideUpdateUI()
    {
        baseUpgradeImage.SetActive(false);
    }


    [Button]
    private void GraveyardOpenAnim()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(graveDoor.transform.DOLocalRotate(new Vector3(0,120,0), graveAnimDuration).SetEase(ease));
        sequence.Append(rightSideGrave.transform.DOLocalMoveX(-1.5f, graveAnimDuration).SetEase(ease));
        sequence.Join(leftSideGrave.transform.DOLocalMoveX(1.5f, graveAnimDuration).SetEase(ease));
        sequence.Join(DOVirtual.DelayedCall(0.25f, () => soulParticle.Play()));
        sequence.Join(DOVirtual.DelayedCall(0.25f, () => shineParticle.Play()));
    }

    private void GraveyardCloseAnim()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(graveDoor.transform.DOLocalRotate(new Vector3(0,0,0), graveAnimDuration).SetEase(ease));
        sequence.Append(rightSideGrave.transform.DOLocalMoveX(-0.6f, graveAnimDuration).SetEase(ease));
        sequence.Join(leftSideGrave.transform.DOLocalMoveX(0.6f, graveAnimDuration).SetEase(ease));
        sequence.Join(DOVirtual.DelayedCall(0.25f, () => soulParticle.Stop()));
        sequence.Join(DOVirtual.DelayedCall(0.5f, () => shineParticle.Stop()));
    }
}