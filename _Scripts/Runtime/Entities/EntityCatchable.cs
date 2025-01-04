using System.Collections;
using __FurtleAll._FurtleScripts.Controllers;
using _Scripts.Runtime.Enums;
using DG.Tweening;
using FurtleGame.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityCatchable : MonoBehaviour, ICatchable
{
    private CaptureScript capture;
    [SerializeField] private Vector3 playerPos;
    public ResourceSO catchableResource;

    [Header("Public")]
    public EntityCatchableAreaController areaController;
    public bool escaping;

    [SerializeField] private float health = 100;

    [Header("Health UI")]
    public TMPColorAnimation textDamageAnim;
    public GameObject catchableHealthCanvas; 
    public Image catchableHealthFiller; 
    public TextMeshProUGUI catchableHealthText;
    public TextMeshProUGUI catchableValueText;
    private bool isHealthUIOpen = false;


    private Coroutine regenRoutine;
    private Coroutine catchingRoutine;

    private const float MAX_HEALTH = 100f;

    public float Health
    {
        get => health;
        set => health = Mathf.Max(0, value);
    }

    public float resistanceValue { get; set; }
    public bool isInDangerZone { get; set; }

    private void Start()
    {
        capture = FindObjectOfType<CaptureScript>();
        SetHealthUI(false); 
    }

    public void ActivateEscapeRig()
    {
        escaping = true;
    }

    public void SetHealthUI(bool toggle)
    {
        float healthPercentage = health / MAX_HEALTH;
        catchableValueText.text = $"{catchableResource.value}";

        if (toggle && !isHealthUIOpen)
        {
            isHealthUIOpen = true;
            catchableHealthCanvas.SetActive(true);

            catchableHealthCanvas.transform.localScale = Vector3.zero; 
            catchableHealthCanvas.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
        else if (!toggle && isHealthUIOpen)
        {
            isHealthUIOpen = false;

            catchableHealthCanvas.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                catchableHealthCanvas.SetActive(false); 
            });
        }

        catchableHealthFiller.DOFillAmount(healthPercentage, 0.2f);
        catchableHealthText.text = $"%{Mathf.RoundToInt(healthPercentage * 100)}";
    }


    public void SetDamageText(bool toggle)
    {
        catchableHealthText.color = toggle ? Color.red : Color.white;
        textDamageAnim.enabled = toggle;
    }

    public void DeactivateEscapeRig()
    {
        escaping = false;
    }

    public void Capture()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(capture.transform.position, 0.25f).SetEase(Ease.OutBounce)); 
        sequence.Join(transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutBounce)); 
        sequence.OnComplete(() =>
        {
            HandleCaptureCompletion();
        });
    }

    private void HandleCaptureCompletion()
    {
        SoundManager.Instance.PlaySound(GameSoundType.CaptureDone);
        capture.CaptureState(false);
        capture.currentCatchablesInRange.Remove(this);
        capture.currentCatchables.Remove(this);
        areaController.RemoveCatchable(this);
        Player.Instance.backpack.Put(catchableResource, catchableResource.capacityValue);
        Player.Instance.experienceDisplay.IncreaseValue(catchableResource.experienceValue);
        Player.Instance.experienceDisplay.Render();
        MiniTaskSystem.Instance.IncreaseStep("CatchOneGear");
        EventManager.TriggerEvent("OnInventoryChanged");
        EventManager.TriggerEvent("OnBackpackUpdate");
        capture.finishParticle.Play(); 
        Destroy(gameObject); 
        QuestManager.Instance.UpdateQuestProgress(catchableResource); 
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public int GetCapacityValue()
    {
        return catchableResource.capacityValue;
    }

    public void OnCatch(float damagePerSecond)
    {
        ActivateEscapeRig();
        if (catchingRoutine == null)
        {
            catchingRoutine = StartCoroutine(Catching(damagePerSecond));
        }

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
            regenRoutine = null;
        }
    }

    public void StartRegen()
    {
        DeactivateEscapeRig();

        if (catchingRoutine != null)
        {
            StopCoroutine(catchingRoutine);
            catchingRoutine = null;
        }

        if (regenRoutine == null)
        {
            regenRoutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator Catching(float givenDamage)
    {
        SetDamageText(true); 

        while (escaping)
        {
            if (Player.Instance.backpack.IsFull())
            {
                yield break;
            }
            
            if (health > 0)
            {
                Health -= givenDamage;
                yield return new WaitForSeconds(0.05f); 
            }
            else
            {
                Capture(); 
                yield break;
            }

            SetHealthUI(true); 
        }
    }

    private IEnumerator RegenerateHealth()
    {
        SetDamageText(false); 

        while (Health < MAX_HEALTH && !escaping)
        {
            Health += 1f; 
            SetHealthUI(true); 
            yield return new WaitForSeconds(0.05f);
        }

        if (Health >= MAX_HEALTH)
        {
            SetHealthUI(false); 
        }

        regenRoutine = null;
    }
}
