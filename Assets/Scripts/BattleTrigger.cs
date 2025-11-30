using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    public Spawner[] spawners;

    [Header("Settings")]
    public bool singleUse = true; // whether trigger runs once
    public bool lockPlayerMovement = true;

    bool triggered = false;

    void Start()
    {
        // auto-find spawners if not assigned
        if (spawners == null || spawners.Length == 0)
        {
            spawners = GetComponentsInChildren<Spawner>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && singleUse) return;

        // detect player - assumes tag "Player" or has FirstPersonPlayer
        var player = other.GetComponent<FirstPersonPlayer>();
        if (player != null)
        {
            triggered = true;
            StartBattle(player);
        }
    }

    void StartBattle(FirstPersonPlayer player)
    {
        // limit player movement
        if (lockPlayerMovement)
        {
            player.allowMovement = false;

            var laneController = player.GetComponent<PlayerLaneController>();
            if (laneController != null)
            {
                laneController.enabled = true;
                laneController.SetLane(1); // center by default
            }

            // TODO: battle UI, music, etc.
        }

        // TODO: disable player attack after battle ends
        var attackController = player.GetComponent<PlayerAttackController>();
        attackController.enabled = true;

        // spawn boss
        GameObject bossGO = null;
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            bossGO = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
        }
        else if (bossPrefab != null)
        {
            bossGO = Instantiate(bossPrefab, transform.position + Vector3.forward * 2f, Quaternion.identity);
        }

        Boss bossComponent = null;
        if (bossGO != null)
        {
            bossComponent = bossGO.GetComponent<Boss>();
        }

        // register spawners with boss
        foreach (var sp in spawners)
        {
            sp.AssignBoss(bossComponent);
            sp.StartSpawning();
        }

        // let boss do initial spawn
        if (bossComponent != null)
        {
            bossComponent.OnBattleStarted(spawners);
        }
    }
}
