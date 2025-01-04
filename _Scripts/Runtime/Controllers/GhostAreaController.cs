using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GhostAreaController : MonoBehaviour
{
    [SerializeField] private int ghostCapacityCount;
    public List<GhostScript> ghostList = new List<GhostScript>();
    public GhostScript ghostPrefab;
    public EntityProp entityProperty;
    public GameObject patrolTargets;
    public bool isInDangerZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInDangerZone = false;
            foreach (var ghost in ghostList)
            {
                ghost.isInDangerZone = false;
                ghost.CheckState();
            }
        }
        if( other.CompareTag("Player"))
        {
            entityProperty.PlayerIsNearState(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            entityProperty.PlayerIsFarState();
        }
    }

    private void Start()
    {
        foreach(var ghost in ghostList)
        {
            if (isInDangerZone)
            {
                ghost.isInDangerZone = true;
                ghost.CheckState();
            }
            else
            {
                ghost.isInDangerZone = false;
                ghost.CheckState();
            }
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnWithDelay());
    }

    private IEnumerator SpawnWithDelay()
    {
        while (ghostList.Count < ghostCapacityCount)
        {
            var randomTime = Random.Range(3f, 8f);
        
            yield return new WaitForSeconds(randomTime);
        
            Spawn();
        }
    }

    public void Spawn()
    {
        if (ghostList.Count >= ghostCapacityCount) return;

        var randomPos = patrolTargets.transform.GetChild(Random.Range(0, patrolTargets.transform.childCount)).position;
    
        var ghost = Instantiate(ghostPrefab, randomPos, Quaternion.identity);
        if (this.isInDangerZone)
        {
            ghost.isInDangerZone = true;
            ghost.CheckState();
        }
        ghost.ghostPatrolTargets = patrolTargets;
        ghost.ghostAreaController = this;
        ghost.transform.SetParent(transform);
        AddGhost(ghost);
    }

    public void AddGhost(GhostScript ghost)
    {
        ghostList.Add(ghost);
    }

    public void RemoveGhost(GhostScript ghost)
    {
        if(ghostList == null)
            return;
        ghostList.Remove(ghost);
        StartSpawning();
    }
}