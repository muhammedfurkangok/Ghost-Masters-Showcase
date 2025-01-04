using UnityEngine;

public interface ICatchable
{
    float Health { get; set; }
    float resistanceValue { get; set; }
    bool isInDangerZone { get; set; }

    void OnCatch(float damagePerSecond);

    void StartRegen();

    void Capture();
    
    Transform GetTransform();
    int  GetCapacityValue();
}