using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedMinion
{
    public GameObject prefab;
    [Range(0, 100)]
    public int weight = 1; // higher = more likely to be chosen
}

public class Boss : MonoBehaviour
{
    private bool isSpawning = false;
    public Animator animator;

    [Header("Minions the boss controls")]
    public List<WeightedMinion> minionPool = new List<WeightedMinion>();

    // (boss can auto-call spawners or do other behavior on start)
    public void OnBattleStarted(Spawner[] spawners)
    {
        foreach (var s in spawners)
        {
            s.SpawnNow();
        }
        // can add boss-specific behaviour like abilities here
    }

    public GameObject GetNextMinionPrefab(Spawner requester)
    {
        if (minionPool == null || minionPool.Count == 0) return null;
        int total = 0;
        foreach (var m in minionPool) total += Mathf.Max(0, m.weight);

        if (total <= 0) return minionPool[0].prefab;

        int r = Random.Range(0, total);
        int running = 0;
        foreach (var m in minionPool)
        {
            if (requester.aliveCount > 0)
            {
                ActorType type = m.prefab.GetComponent<Minion>().enemyType;
                if(type == ActorType.Ranged)
                {
                    continue;
                }
            }
            running += Mathf.Max(0, m.weight);
            if (r < running)
            {
                return m.prefab;
            }
        }

        return minionPool[minionPool.Count - 1].prefab;
    }

    // for more control: spawn directly
    /*public void SpawnMinionAt(Vector3 position)
    {
        var prefab = GetNextMinionPrefab(null);
        if (prefab != null)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
    }*/
}

