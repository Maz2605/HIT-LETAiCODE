using UnityEngine;
using System.Collections.Generic;
using DesignPattern;
using pooling;

public class CloneManager : Singleton<CloneManager>
{
    public GameObject clonePrefab;    
    public int maxClones = 3;

    private int spawnRemaining;
    private List<GameObject> clones = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        spawnRemaining = maxClones;
    }

    public bool CanSpawn()
    {
        return spawnRemaining > 0;
    }

    public GameObject SpawnClone(Vector3 position)
    {
        if (!CanSpawn())
            return null;

        GameObject clone = PoolingManager.Spawn(clonePrefab, position, Quaternion.identity);

        clones.Add(clone);
        spawnRemaining--;

        return clone;
    }

    public void RemoveOldest()
    {
        if (clones.Count == 0) return;

        GameObject oldest = clones[0];
        clones.RemoveAt(0);

        PoolingManager.Despawn(oldest);
        spawnRemaining++;
    }

    public void RemoveCloneExact(GameObject clone)
    {
        if (clones.Remove(clone))
        {
            PoolingManager.Despawn(clone);
            spawnRemaining++;
        }
    }

    public void DestroyCloneDeath(GameObject clone)
    {
        if (clones.Remove(clone))
        {
            PoolingManager.Despawn(clone);
            spawnRemaining++;
        }
    }


    public void ResetAllClones()
    {
        foreach (var c in clones)
        {
            if (c != null)
                PoolingManager.Despawn(c);
        }

        clones.Clear();
        spawnRemaining = maxClones;
    }
}