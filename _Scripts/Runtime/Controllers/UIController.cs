using _Scripts.Runtime.Enums;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject achievementPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button upgradeCloseButton;
    
    [SerializeField] private Button achievementButton;
    [SerializeField] private Button achievementCloseButton;

    private void Start()
    {
        upgradePanel.transform.localScale = Vector3.zero; 
        achievementPanel.transform.localScale = Vector3.zero;

        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        upgradeCloseButton.onClick.AddListener(CloseUpgradePanel);
        achievementButton.onClick.AddListener(OpenAchievementPanel);
        achievementCloseButton.onClick.AddListener(CloseAchievementPanel);
    }

    public void OpenUpgradePanel()
    {
        buttonClickSound();
        upgradePanel.SetActive(true);
        upgradePanel.transform.DOScale(Vector3.one, 0.35f) 
            .SetEase(Ease.OutQuart); 
    }

    public void CloseUpgradePanel()
    {
        buttonClickSound();
        upgradePanel.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InCubic) 
            .OnComplete(() => upgradePanel.SetActive(false));
    }

    public void OpenAchievementPanel()
    {
        buttonClickSound();
        achievementPanel.SetActive(true);
        achievementPanel.transform.DOScale(Vector3.one, 0.35f)
            .SetEase(Ease.OutQuart);
    }

    public void CloseAchievementPanel()
    {
        buttonClickSound();
        achievementPanel.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => achievementPanel.SetActive(false));
    }
    
    public void buttonClickSound()
    {
        SoundManager.Instance.PlaySound(GameSoundType.ButtonClick);
    }

    private void OnDisable()
    {
        upgradeButton.onClick.RemoveListener(OpenUpgradePanel);
        upgradeCloseButton.onClick.RemoveListener(CloseUpgradePanel);
        achievementButton.onClick.RemoveListener(OpenAchievementPanel);
        achievementCloseButton.onClick.RemoveListener(CloseAchievementPanel);
    }
}