using UnityEngine;
using System.Collections.Generic;
using DesignPattern;

public class CloneManager : Singleton<CloneManager>
{
    public GameObject clonePrefab;    
    public int maxClones = 3;

    public int spawnRemaining;
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

        GameObject clone = Instantiate(clonePrefab, position, Quaternion.identity);

        clones.Add(clone);

        // UI
        spawnRemaining--;

        return clone;
    }

    public void RemoveOldest()
    {
        if (clones.Count == 0) return;

        GameObject oldest = clones[0];
        clones.RemoveAt(0);

        if (oldest != null)
            Destroy(oldest);

        spawnRemaining++;

        UIManager.Instance
            .GetUI<UIGamePlay>("UIGamePlay")
            .RecoverTimeItem(spawnRemaining - 1);
    }

    public void RemoveCloneExact(GameObject clone)
    {
        if (clones.Remove(clone))
        {
            if (clone != null)
                Destroy(clone);

            spawnRemaining++;
        }
    }

    public void DestroyCloneDeath(GameObject clone)
    {
        if (clones.Remove(clone))
        {
            if (clone != null)
                Destroy(clone);

            spawnRemaining++;

            UIManager.Instance
                .GetUI<UIGamePlay>("UIGamePlay")
                .RecoverTimeItem(spawnRemaining - 1);
        }
    }

    public void ResetAllClones()
    {
        foreach (var c in clones)
        {
            if (c != null)
                Destroy(c);
        }

        clones.Clear();

        spawnRemaining = maxClones;

        UIManager.Instance
            .GetUI<UIGamePlay>("UIGamePlay")
            .ResetAllItems();
    }
}
