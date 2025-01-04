using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityCatchableAreaController : MonoBehaviour
{
    public EntityCatchable catchablePrefab;
    public List<EntityCatchable> catchableList = new List<EntityCatchable>();

    [SerializeField] private int catchableCapacityCount;
    [SerializeField] private GameObject spawnPoints;
    
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;

    public void StartSpawning()
    {
        StartCoroutine(SpawnWithDelay());
    }

    private IEnumerator SpawnWithDelay()
    {
        while (catchableList.Count < catchableCapacityCount)
        {
            var randomTime = Random.Range(minSpawnTime, maxSpawnTime);
        
            yield return new WaitForSeconds(randomTime);
        
            Spawn();
        }
    }

    public void Spawn()
    {
        if (catchableList.Count >= catchableCapacityCount) return;

        var randomPos = spawnPoints.transform.GetChild(Random.Range(0, spawnPoints.transform.childCount)).position;
        var catchable = Instantiate(catchablePrefab, randomPos, Quaternion.identity);
        catchable.transform.localScale = Vector3.zero;
        catchable.areaController = this;
        catchable.transform.SetParent(transform);
        catchable.transform.DOScale( Vector3.one, 0.5f);
        AddCatchable(catchable);
    }

    public void AddCatchable(EntityCatchable catchable)
    {
        catchableList.Add(catchable);
    }

    public void RemoveCatchable(EntityCatchable catchable)
    {
        catchableList.Remove(catchable);
        StartSpawning();
    }

}
