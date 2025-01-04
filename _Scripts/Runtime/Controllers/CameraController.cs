using System;
using Cinemachine;
using Runtime.Managers;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float fovSpeed = 5f;  
    [SerializeField] private float minFov = 60f;  
    [SerializeField] private float maxFov = 90f;   
    [SerializeField] private float fovLerpSpeed = 2f;  

    private void Start()
    {
        if (cinemachineVirtualCamera == null)
        {
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
    }

    private void Update()
    {
        AdjustFovBasedOnInput();
    }

    private void AdjustFovBasedOnInput()
    {
        var input = InputManager.Instance.GetMovementInput();
        if (Mathf.Abs(input.x) > 0 || Mathf.Abs(input.z) > 0)
        {
            float fovChange = Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.z)) * fovSpeed * Time.deltaTime / 2;

            float newFov = Mathf.Clamp(cinemachineVirtualCamera.m_Lens.FieldOfView + fovChange, minFov, maxFov);

            cinemachineVirtualCamera.m_Lens.FieldOfView = newFov;
        }
        else
        {
            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
                cinemachineVirtualCamera.m_Lens.FieldOfView, 
                minFov, 
                fovLerpSpeed * Time.deltaTime
            );
        }
    }
}