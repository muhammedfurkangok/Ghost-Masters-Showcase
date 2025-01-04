using System.Collections;
using System.Collections.Generic;
using __FurtleAll._FurtleScripts.Controllers;
using _Scripts.Runtime.Enums;
using DG.Tweening;
using FurtleGame.EventSystem;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class GhostScript : MonoBehaviour, ICatchable
{
    [Header("Public")] public FSMOwner ghostFsm;
    public SphereCollider ghostCollider;
    public ResourceSO ghostResource;
    public GameObject ghostMesh;
    public GameObject ghostPatrolTargets;
    public Material ghostMaterial;
    public Material ghostDangerMaterial;
    public GhostAreaController ghostAreaController;
    public float resistance;
    public bool escaping;
    public bool isItSafeAreaBefore;
    public string miniTaskKey;

    [Header("Health UI")] public TMPColorAnimation textDamageAnim;
    public TextMeshProUGUI ghostHealthText;
    public TextMeshProUGUI ghostValueText;
    public GameObject ghostHealthCanvas;
    public Image ghostHealthFiller;

    [Header("Serialized Variables")] [SerializeField]
    private float currentHealth;

    [SerializeField] private float passiveStateHealth;
    [SerializeField] private float aggressiveStateHealth;

    private bool isHealthUIOpen = false;


    [Header("Private Variables")] private Vector3 playerPos;
    private NavMeshAgent ghostNavmeshAgent;
    private CaptureScript capture;
    private Coroutine regenRoutine;
    private Coroutine catchingRoutine;

    private const float AGGRESSIVE_SCALE = 2.5f;
    private const float PASSIVE_SCALE = 1.75f;
    private float MAX_HEALTH;

    public float Health
    {
        get => currentHealth;
        set => currentHealth = Mathf.Max(0, value);
    }

    public float resistanceValue
    {
        get => resistance;
        set => resistance = value;
    }

    public bool isInDangerZone { get; set; }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        capture = FindObjectOfType<CaptureScript>();
        ghostNavmeshAgent = GetComponent<NavMeshAgent>();
        ghostCollider = GetComponent<SphereCollider>();
        MAX_HEALTH = currentHealth;
        SetFsmTargetList();
    }

    public void CheckState()
    {
        if (isInDangerZone)
        {
            AgressiveState();
        }
        else
        {
            PassiveState();
        }
    }


    private void AgressiveState()
    {
        currentHealth = aggressiveStateHealth;
        MAX_HEALTH = currentHealth;

        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one * AGGRESSIVE_SCALE, 1f).SetEase(Ease.OutBounce));
        sequence.Join(ghostMesh.GetComponent<SkinnedMeshRenderer>().materials[0]
            .DOColor(ghostDangerMaterial.color, 1f));
    }

    private void PassiveState()
    {
        if (isItSafeAreaBefore) return;
        currentHealth = passiveStateHealth;
        MAX_HEALTH = currentHealth;
        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one * PASSIVE_SCALE, 1f).SetEase(Ease.OutBounce));
        sequence.Join(ghostMesh.GetComponent<SkinnedMeshRenderer>().materials[0].DOColor(ghostMaterial.color, 1f));
        sequence.AppendCallback(() => isItSafeAreaBefore = true);
    }

    public void OnCatch(float damagePerSecond)
    {
        ActivateEscapeRig();
        SetFsmBool(GhostFsmState.IsPatrolling, false);
        SetFsmBool(GhostFsmState.IsPlayerInRange, true);

        if (catchingRoutine == null)
        {
            catchingRoutine = StartCoroutine(Catching(damagePerSecond));
        }

        StopAndNullifyCoroutine(ref regenRoutine);
    }

    public void StartRegen()
    {
        DeactivateEscapeRig();

        SetFsmBool(GhostFsmState.IsPlayerInRange, false);
        SetFsmBool(GhostFsmState.IsPatrolling, true);

        StopAndNullifyCoroutine(ref catchingRoutine);

        if (regenRoutine == null)
        {
            regenRoutine = StartCoroutine(RegenerateHealth());
        }
    }

    public void ActivateEscapeRig()
    {
        //chestGhostMesh.SetActive(false);
        //tailGhostMesh.SetActive(true);
        // tailAnimator.SetTrigger("escape");
        escaping = true;
    }

    public void DeactivateEscapeRig()
    {
        //chestGhostMesh.SetActive(true);
        //tailGhostMesh.SetActive(false);
        escaping = false;
    }

    public void SetHealthUI(bool toggle)
    {
        float healthPercentage = currentHealth / MAX_HEALTH;
        ghostValueText.text = $"{ghostResource.value}";

        if (toggle && !isHealthUIOpen)
        {
            isHealthUIOpen = true;
            ghostHealthCanvas.SetActive(true);

            ghostHealthCanvas.transform.localScale = Vector3.zero;
            ghostHealthCanvas.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
        else if (!toggle && isHealthUIOpen)
        {
            isHealthUIOpen = false;

            ghostHealthCanvas.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ghostHealthCanvas.SetActive(false);
            });
        }

        ghostHealthFiller.DOFillAmount(healthPercentage, 0.2f);
        ghostHealthText.text = $"%{Mathf.RoundToInt(healthPercentage * 100)}";
    }


    public void SetDamageText(bool toggle)
    {
        ghostHealthText.color = toggle ? ghostHealthText.color : Color.white;
        textDamageAnim.enabled = toggle;
    }

    public void Capture()
    {
        PlayCaptureAnimation().OnComplete(HandleGameCaptureState);
    }

    private Tween PlayCaptureAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => ghostNavmeshAgent.enabled = false);
        sequence.Append(transform.DOMove(capture.transform.position, .25f).SetEase(Ease.OutBounce));
        sequence.Join(transform.DOScale(Vector3.zero, .25f).SetEase(Ease.OutBounce));
        return sequence;
    }

    private void HandleGameCaptureState()
    {
        SoundManager.Instance.PlaySound(GameSoundType.CaptureDone);
        ghostCollider.enabled = false;
        ghostFsm.enabled = false;
        capture.bulgeController.CreateBulgeEffect(0f);
        Player.Instance.backpack.Put(ghostResource, ghostResource.capacityValue);
        Player.Instance.experienceDisplay.IncreaseValue(ghostResource.experienceValue);
        Player.Instance.experienceDisplay.Render();
        capture.CaptureState(false);
        if (ghostAreaController != null)
        {
            ghostAreaController.RemoveGhost(this);
        }

        ghostNavmeshAgent.enabled = false;

        capture.currentCatchablesInRange.Remove(this);
        capture.currentCatchables.Remove(this);

        EventManager.TriggerEvent("OnInventoryChanged");
        EventManager.TriggerEvent("OnBackpackUpdate");
        capture.finishParticle.Play();
        MiniTaskSystem.Instance.IncreaseStep(miniTaskKey);
        QuestManager.Instance.UpdateQuestProgress(ghostResource);
        DOVirtual.DelayedCall(0.25f, () => Destroy(gameObject));
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public int GetHalf(int value)
    {
        return value / 2;
    }

    public int GetCapacityValue()
    {
        return ghostResource.capacityValue;
    }

    public void SetFsmTargetList()
    {
        var fsmTargetList = ghostFsm.graph.blackboard.GetVariableValue<List<GameObject>>("TargetList");

        foreach (var patrolWayPoint in ghostPatrolTargets.transform.GetComponentsInChildren<Transform>())
        {
            fsmTargetList.Add(patrolWayPoint.gameObject);
        }

        ghostFsm.graph.blackboard.SetVariableValue("TargetList", fsmTargetList);
    }

    public void SetFsmBool(GhostFsmState state, bool value)
    {
        ghostFsm.graph.blackboard.SetVariableValue(state.ToString(), value);
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


            if (currentHealth > 0)
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

    private void StopAndNullifyCoroutine(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}

public enum GhostFsmState
{
    IsPatrolling,
    IsPlayerInRange,
    IsInDangerZone,
}