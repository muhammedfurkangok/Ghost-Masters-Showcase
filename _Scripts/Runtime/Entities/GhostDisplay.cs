using _Scripts.Runtime.Enums;
using DG.Tweening;
using FurtleGame.EventSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class GhostDisplay : ValueDisplay
{
    [SerializeField] private Transform transformPosition;
    [SerializeField] private Button displayButton;

    private Tween displayTween;
    
    protected override void Start()
    {
        base.Start();
        displayButton.onClick.AddListener(OnDisplayButtonClicked);
    }

    private void OnDisplayButtonClicked()
    {
        SoundManager.Instance.PlaySound(GameSoundType.ButtonClick);
        displayTween?.Kill();
        displayButton.transform.localScale = Vector3.one * 1.25f;
        displayTween = displayButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f, 10, 1);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        displayButton.onClick.RemoveListener(OnDisplayButtonClicked);
    }
    
    public override string Format(float number)
    {
        if (number >= 1000000)
            return Mathf.Round(number/1000000) + "M";
        else if (number >= 10000)
            return Mathf.Round(number/1000) + "K";
        else
            return Mathf.Round(number).ToString();
    }
    public override float GetValue()
    {
        return SaveData.Ghost;
    }

    public override void IncreaseValue(float value)
    {
        SaveData.Ghost += (int)value;
        EventManager.TriggerEvent("OnGhostChanged", value);
    }
    
}
