using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Runtime.Enums;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeAreaManager : MonoBehaviour
{
    [SerializeField] private Button shopButton;
    [SerializeField] private GameObject exchangeCanvas;
    [SerializeField] private GameObject exchangeButton;
    [SerializeField] private ParticleSystem flueSmoke;
    [SerializeField] private GameObject windArrow;
    [SerializeField] private GameObject windDirections;

    private void Start()
    {
        shopButton.onClick.AddListener(OnShopButtonClicked);

    }
    private void OnShopButtonClicked()
    {
        exchangeCanvas.SetActive(true);
        
        SoundManager.Instance.PlaySound(GameSoundType.WitchShop);
    }

    private void OnDisable()
    {
        shopButton.onClick.RemoveListener(OnShopButtonClicked);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RevealUpdateUI();
            StartEnviromentAnims();
        }
    }

    private void StartEnviromentAnims()
    {
        windArrow.transform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental);
        windDirections.transform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental);
        flueSmoke.Play();
    }

    private void StopEnvironmentAnims()
    {
        windArrow.transform.DOKill();
        windDirections.transform.DOKill();
        flueSmoke.Stop();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideUpdateUI();
            StopEnvironmentAnims();
        }
    }


    private void RevealUpdateUI()
    {
        exchangeButton.SetActive(true);
    }

    private void HideUpdateUI()
    {
        exchangeButton.SetActive(false);
    }
}

 