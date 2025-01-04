using UnityEngine;
using System.Collections.Generic;
using __FurtleAll._FurtleScripts.Controllers;
using _Scripts.Runtime.Enums;
using DG.Tweening;
using FurtleGame.EventSystem;
using FurtleGame.UpgradeSystem;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class CaptureScript : MonoBehaviour
{
    [ShowInInspector] public List<ICatchable> currentCatchablesInRange;
    [ShowInInspector] public List<ICatchable> currentCatchables;

    private ICatchable closestCatchable;

    public BulgeController bulgeController;

    public ParticleSystem captureParticle;
    public ParticleSystem captureWhirlParticle;
    public ParticleSystem finishParticle;

    public bool capturing;

    [SerializeField] private float captureTurnSpeed = 100;
    public int currentCatchableLimit = 1;
    public int maxCatchableLimit = 4;
    public int minCatchableLimit = 1;
    public Upgrade catchableLimitUpgrade;

    public float currentDamage;
    public int baseDamage = 1;
    public int maxDamage = 10;
    public Upgrade damageUpgrade;


    private void OnEnable()
    {
        EventManager.StartListening("OnUpgrade", OnUpgrade);
    }

    private void Start()
    {
        currentCatchables = new List<ICatchable>();
        currentCatchablesInRange = new List<ICatchable>();
        bulgeController = GetComponent<BulgeController>();
        OnUpgrade(null);
    }

    private void OnUpgrade(Upgrade upgrade)
    {
        currentDamage = damageUpgrade ? damageUpgrade.Evaluate(baseDamage, maxDamage) : currentDamage;
        currentCatchableLimit = (int)(catchableLimitUpgrade
            ? catchableLimitUpgrade.Evaluate(minCatchableLimit, maxCatchableLimit)
            : currentCatchableLimit);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnUpgrade", OnUpgrade);
    }


    private void FixedUpdate()
    {
        if (currentCatchablesInRange.Count > 0)
        {
            GetClosestCatchables();
        }

        if (capturing)
        {
                Player.Instance.playerMovementController.rotateLocked = true;
                RotateTowardsCatchable(currentCatchables[0].GetTransform().position);
        }
        else
        {
            Player.Instance.playerMovementController.rotateLocked = false;
        }

        // if (capturing)
        // {
        //     captureCanvas.SetActive(true);
        // }
        // else
        // {
        //     captureCanvas.SetActive(false);
        // }
    }

    private void RotateTowardsCatchable(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - Player.Instance.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion smoothRotation = Quaternion.Slerp(Player.Instance.transform.rotation, targetRotation,
            Time.fixedDeltaTime * captureTurnSpeed);
        smoothRotation.x = 0;
        smoothRotation.z = 0;
        smoothRotation = Quaternion.Normalize(smoothRotation);
        Player.Instance.transform.rotation = smoothRotation;
    }


    private void GetClosestCatchables()
    {
        float minDistance = float.MaxValue;
        closestCatchable = null;

        foreach (var catchable in currentCatchablesInRange)
        {
            var catchableTransform = catchable.GetTransform();
            float distance = Vector3.Distance(transform.position, catchableTransform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCatchable = catchable;
            }
        }

        if (closestCatchable != null && currentCatchables.Count < currentCatchableLimit)
        {
            if (currentCatchablesInRange.Contains(closestCatchable))
            {
                currentCatchablesInRange.Remove(closestCatchable);
            }

            currentCatchables.Add(closestCatchable);
            StartCapture();
        }
    }

    public void StartCapture()
    {
        if (currentCatchables.Count > 0)
        {
            var currentCatchableCapacityValue = currentCatchables[0].GetCapacityValue();
            if (Player.Instance.backpack.IsHaveEnoughSpace(currentCatchableCapacityValue))
            {
                foreach (var catchable in currentCatchables)
                {
                    catchable.OnCatch(currentDamage);
                }

                CaptureState(true);
            }
        }
    }

    public void CaptureState(bool state)
    {
        capturing = state;


        if (state)
        {
            SoundManager.Instance.PlaySound(GameSoundType.Capture);
            captureParticle.Play();
            captureWhirlParticle.Play();
        }
        else
        {
            if (SoundManager.Instance.audioSource.isPlaying)
                SoundManager.Instance.audioSource.Stop();
            captureParticle.Stop();
            captureWhirlParticle.Stop();
        }
    }
}