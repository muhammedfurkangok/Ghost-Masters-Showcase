using System;
using System.Collections;
using DG.Tweening;
using EasyTransition;
using FurtleGame.EventSystem;
using FurtleGame.UpgradeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public int currentHealth;
    public int baseHealth = 100;
    public int maxHealth = 200;
    public Upgrade healthUpgrade;

    public CanvasGroup dangerZoneCanvas;
    public GameObject playerHealthCanvas;
    public Image playerHealthFiller;
    public TextMeshProUGUI playerHealthText;
    public TMPColorAnimation textDamageAnim;
   

    [SerializeField] private bool isInSafeZone;

    private Coroutine damageRoutine;
    private Coroutine regenRoutine;


    private void OnEnable()
    {
        EventManager.StartListening("OnUpgrade", OnUpgrade);
    }

    private void Start()
    {
        playerHealthCanvas.transform.localScale = Vector3.one * 2f;
        CalculateHealth();
    }


    private void OnUpgrade(Upgrade upgrade)
    {
        CalculateHealth();
    }

    private void CalculateHealth()
    {
        currentHealth = (int)(healthUpgrade ? healthUpgrade.Evaluate(baseHealth, maxHealth) : baseHealth);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnUpgrade", OnUpgrade);
    }

    private void OnOutsideSafeZone()
    {
      dangerZoneCanvas.DOFade(1, 1f);
      SoundManager.Instance.SetAllAudioSourcesAudioLowPassFilterLow();
    }
    
    private void OnInsideSafeZone()
    {
         dangerZoneCanvas.DOFade(0, 1f);
        SoundManager.Instance.SetAllAudioSourcesAudioLowPassFilterHigh();
    }
    
    private void Update()
    {
        if (!isInSafeZone)
        {
            if (damageRoutine == null && currentHealth > 0)
            {
                regenRoutine = null; 
                damageRoutine = StartCoroutine(Damage(1));
            }
        }
        else
        {
            if (regenRoutine == null && currentHealth < baseHealth)
            {
                if (damageRoutine != null) 
                {
                    StopCoroutine(damageRoutine);
                    damageRoutine = null;
                }
                regenRoutine = StartCoroutine(RegenerateHealth());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = true;
            OnInsideSafeZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = false;
            OnOutsideSafeZone();
            if (regenRoutine != null)
            {
                StopCoroutine(regenRoutine);
                regenRoutine = null;
            }
        }
    }

    public void SetHealthUI(bool toggle)
    {
        playerHealthCanvas.SetActive(toggle);
        playerHealthFiller.DOFillAmount(currentHealth / (float)baseHealth, 0.2f);
        playerHealthText.text = $"{currentHealth}";

        if (currentHealth <= baseHealth * 0.75f)
        {
            playerHealthCanvas.transform.DOScale(2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            playerHealthFiller.DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            playerHealthCanvas.transform.DOKill();
            playerHealthCanvas.transform.DOScale( 1.5f, 0.5f); 
            playerHealthFiller.DOKill();
            playerHealthFiller.color = Color.green;
        }
    }


    public void SetDamageText(bool toggle)
    {
        playerHealthText.color = toggle ? playerHealthText.color : Color.white;
        textDamageAnim.enabled = toggle;
    }

    private IEnumerator Damage(int givenDamage)
    {
        SetDamageText(true);
        while (currentHealth > 0)
        {
            currentHealth -= givenDamage;
            SetHealthUI(true);

            if (currentHealth <= baseHealth * 0.25f)
            {
                playerHealthCanvas.transform.DOScale(2f, 0.1f).SetLoops(2, LoopType.Yoyo);
            }
            yield return new WaitForSeconds(0.05f);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                SetHealthUI(false);
                PlayerDeath();
                yield break;
            }
        }
        damageRoutine = null;
    }


    private void PlayerDeath()
    {
        EventManager.TriggerEvent("OnPlayerDeath");
    }

    private IEnumerator RegenerateHealth()
    {
        SetDamageText(false);
        while (currentHealth < baseHealth)
        {
            currentHealth += 1;
            SetHealthUI(true);
            yield return new WaitForSeconds(0.05f);
        }

        regenRoutine = null;
        if (currentHealth >= baseHealth)
        {
            SetHealthUI(false); 
        }
    }
}
