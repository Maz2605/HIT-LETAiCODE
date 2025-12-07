using UnityEngine;
using System.Collections.Generic;
using DesignPattern;

public class CloneManager : Singleton<CloneManager>
{
    public int maxClones = 3;  
    private int spawnRemaining;

    List<GameObject> clones = new List<GameObject>();
    protected override void Awake()
    {
        base.Awake();
        spawnRemaining = maxClones;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RemoveOldest();
        }
    }


    public bool CanSpawn()
    {
        return spawnRemaining > 0;
    }

    public void RegisterClone(GameObject clone)
    {
        clones.Add(clone);
        spawnRemaining--;
    }

    public void RemoveOldest()
    {
        if (clones.Count == 0) return;

        GameObject oldest = clones[0];
        clones.RemoveAt(0);
        Destroy(oldest);

        spawnRemaining++; 
    }
    public void RemoveCloneExact(GameObject clone)
    {
        if (clones.Contains(clone))
        {
            clones.Remove(clone);
        }
    }

    public void DestroyCloneDeath(GameObject clone)
    {
        Destroy(clone);
        spawnRemaining++;
    }
}