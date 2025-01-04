using System;
using __FurtleAll._FurtleScripts.Controllers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using FurtleGame.EventSystem;
using FurtleGame.UpgradeSystem;
using Runtime.Managers;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Player Settings")] public Player player;
    public Upgrade speedUpgrade;
    public float baseSpeed = 7.5f;
    public float maxSpeed = 10f;
    public float turnSpeed = 10f;
    public bool shouldSmoothTurns;
    public bool rotateLocked;
    public AnimationCurve speedCurve;

    [Header("Events")] public UnityEvent onMovementStarted;
    public UnityEvent onMovementStopped;

    public bool IsStationary => InputManager.Instance.GetMovementInput() == Vector3.zero;

    [Header("Capture Settings")] public CaptureScript captureScript;
    public float pullStrength = 10f;

    [Header("Physics Settings")] public Rigidbody playerRb;

    private bool stopped = true;
    private Vector3 direction;
    public float currentSpeed;
    private Animator animator;
    public bool locked;
    private NavMeshAgent agent;
    private Transform mainCam;
    private float boostMultiplier = 1f;


    private void OnEnable()
    {
        EventManager.StartListening("OnUpgrade", OnUpgrade);
    }

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        CalculateSpeed();
    }

    private void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        captureScript = GetComponent<CaptureScript>();
        mainCam = Camera.main.transform;
    }


    private void OnDisable()
    {
        EventManager.StopListening("OnUpgrade", OnUpgrade);
    }

    private void OnUpgrade(Upgrade upgrade)
    {
        CalculateSpeed();
    }

    void FixedUpdate()
    {
        if (locked) return;

        direction = InputManager.Instance.GetMovementInput();


        HandleNormalMovement();
    }

    private void HandleNormalMovement()
    {
        if (direction == Vector3.zero)
        {
            if (!stopped)
            {
                stopped = true;
                onMovementStopped.Invoke();
                animator.SetBool("Moving", false);
            }

            return;
        }

        animator.SetFloat("Speed", speedCurve.Evaluate(direction.magnitude) * baseSpeed / 4f);

        if (stopped)
        {
            stopped = false;
            onMovementStarted.Invoke();
            animator.SetBool("Moving", true);
        }


        Vector3 finalDirection = RotatePlayer();


        transform.position += finalDirection * (currentSpeed * Time.fixedDeltaTime * boostMultiplier);
    }


    private Vector3 RotatePlayer()
    {
        Vector3 faceDirection = new Vector3(mainCam.forward.x, 0, mainCam.forward.z);
        float cameraAngle = Vector3.SignedAngle(Vector3.forward, faceDirection, Vector3.up);
        Vector3 finalDirection = Quaternion.Euler(0, cameraAngle, 0) * direction;


        if (shouldSmoothTurns)
        {
            if (!rotateLocked)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(finalDirection),
                    Time.fixedDeltaTime * turnSpeed);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(finalDirection);
        }

        finalDirection.y = 0;

        return finalDirection;
    }

    public void CalculateSpeed()
    {
        currentSpeed = speedUpgrade ? speedUpgrade.Evaluate(baseSpeed, maxSpeed) : baseSpeed;
    }

    public void SetBoostMultiplier(float multiplier)
    {
        boostMultiplier = multiplier;
    }
}