using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;      
    public float spawnInterval = 2f;    
    public int maxEnemies = 1;         

    private int currentEnemies = 0;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies)
            return;

        Instantiate(enemyPrefab, transform.position, transform.rotation);
        currentEnemies++;
    }
}