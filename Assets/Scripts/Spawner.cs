using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Spawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public Transform spawnPoint;
    public float spawnIntervalMin = 1.5f;
    public float spawnIntervalMax = 5f;
    public int maxTotalSpawns = 10;
    public int maxAliveAtOnce = 4;  // concurrency limit

    [Header("Debug")]
    public bool autoStart = false;

    Boss boss; // assigned by BattleTrigger
    int totalSpawned = 0;
    public int aliveCount = 0;
    bool spawning = false;
    Coroutine spawnRoutine;
    private Transform[] waypoints;

    void Awake()
    {
        // Gather waypoints from lane parent
        waypoints = transform.parent
            .GetComponentsInChildren<Waypoint>()
            .Select(w => w.transform)
            .OrderBy(w => w.name)
            .ToArray();

        if (waypoints.Length == 0)
        {
            Debug.LogError($"Spawner '{name}' cannot find waypoints in parent lane!");
        }

        if (spawnPoint == null)
            spawnPoint = transform; // fallback
    }

    public void AssignBoss(Boss b)
    {
        boss = b;
    }

    public void StartSpawning()
    {
        if (!spawning)
        {
            spawning = true;
            spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawning()
    {
        if (spawning)
        {
            spawning = false;
            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        }
    }

    IEnumerator SpawnLoop()
    {
        boss.animator.SetBool("isSpawning", true);
        while (spawning && (maxTotalSpawns <= 0 || totalSpawned < maxTotalSpawns))
        {
            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(wait);

            if (maxAliveAtOnce > 0 && aliveCount >= maxAliveAtOnce)
            {
                continue;
            }

            // get minion prefab from boss
            GameObject prefabToSpawn = null;
            if (boss != null)
            {
                prefabToSpawn = boss.GetNextMinionPrefab(this);
            }

            if (prefabToSpawn == null)
            {
                Debug.LogWarning("Spawner: No minion prefab provided by Boss; skipping spawn.");
                continue;
            }

            GameObject go = Instantiate(prefabToSpawn, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
            totalSpawned++;
            aliveCount++;

            // TODO: monster death callback
            var monster = go.GetComponent<Minion>();
            if (monster != null)
            {
                monster.Initialize(waypoints); // Pass waypoints to monster
                monster.onDestroy += () => { aliveCount = Mathf.Max(0, aliveCount - 1); };
            }

            // respect total limit
            if (maxTotalSpawns > 0 && totalSpawned >= maxTotalSpawns)
            {
                spawning = false;
                break;
            }
        }
        boss.animator.SetBool("isSpawning", false);
    }

    // helper to force spawn now
    public void SpawnNow()
    {
        if (boss != null)
        {
            GameObject prefabToSpawn = boss.GetNextMinionPrefab(this);
            if (prefabToSpawn != null)
            {
                GameObject go = Instantiate(prefabToSpawn, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
                totalSpawned++;
                aliveCount++;
                var monster = go.GetComponent<Minion>();
                if (monster != null)
                {
                    monster.Initialize(waypoints); // Pass waypoints to monster
                    monster.onDestroy += () => { aliveCount = Mathf.Max(0, aliveCount - 1); };
                }
            }
        }
    }

    // For debug
    void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }
}
