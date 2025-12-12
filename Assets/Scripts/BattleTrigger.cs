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

    public GameObject light_1;
    public GameObject light_2;

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
        
        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartBattle(other.gameObject);

            light_1.SetActive(true);
            light_2.SetActive(true);
        }
    }

    void StartBattle(GameObject player)
    {
        Debug.Log("Initiating Battle");

        // Start battle music and play battle begin SFX
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartBattle();
        }

        // limit player movement
        if (lockPlayerMovement)
        {
            var playerScript = player.GetComponent<Player>();
            playerScript.switchToBattle();
        }

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
