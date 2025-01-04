using System;
using DG.Tweening;
using FurtleGame.EventSystem;
using FurtleGame.UpgradeSystem;
using Runtime.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class Backpack : Storage
{
    public LiquidVolumeAnimator liquidVolumeAnimator;
    public int capacity;
    public int baseCapacity = 6;
    public int maxCapacity = 12;
    
    public Upgrade capacityUpgrade;


    
    private void OnEnable()
    {
        EventManager.StartListening("OnUpgrade", OnUpgrade);
    }

    private void Start()
    {
        capacity = capacityUpgrade ? (int)capacityUpgrade.Evaluate(baseCapacity, maxCapacity, maxCapacity) : baseCapacity;
    }
    
    public void UpdateLiquidVolume(int totalAmount)
    {
        float targetLevel = Mathf.Clamp01((float)totalAmount / capacity);
        DOTween.To(() => liquidVolumeAnimator.level, x => liquidVolumeAnimator.level = x, targetLevel, 0.5f); // 0.5f is the duration
    }


    private void OnUpgrade(Upgrade upgrade)
    {
        capacity = capacityUpgrade ? (int)capacityUpgrade.Evaluate(baseCapacity, maxCapacity, maxCapacity) : baseCapacity;
        EventManager.TriggerEvent("OnInventoryChanged");
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnUpgrade", OnUpgrade);
    }
    public bool HasSpace()
    {
        return GetFreeSpace() > 0;
    }

    public bool HasItem()
    {
        return GetTotalAmount() > 0;
    }

    public bool IsFull()
    {
        int total = 0;
        foreach (var resource in resources)
        {
            total += resource.Value;
        }

        return total >= capacity;
    }

    public bool IsHaveEnoughSpace(int amount)
    {
        return GetFreeSpace() >= amount;
    }
    
    
    public int GetFreeSpace()
    {
        return (int)capacity - GetTotalAmount();
    }

    public int GetTotalAmount()
    {
        int total = 0;
        foreach (var resource in resources)
        {
            total += resource.Value;
        }

        return total;
    }

    public override bool Put(ResourceSO resource, int amount = 1)
    {
        if (IsFull())
        {
            return false;
        }
        return base.Put(resource, amount);
    }

    public override int Get(ResourceSO resource, int amount = 1)
    {
        if (IsEmpty())
        {
            Debug.Log("Backpack is empty");
            return 0;
        }

        return base.Get(resource, amount);
    }
}