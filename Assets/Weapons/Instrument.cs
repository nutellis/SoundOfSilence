using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Instrument : MonoBehaviour
{
    public int id;
    public string instrumentName;
    public float attackCooldown = 0.5f;
    public int projectileSpeed;
    public int attackDamage = 10;
    public float attackRange = 0.5f;

    public float attackRate;

    public Transform projectilePosition;
    public GameObject projectileSpawn;

    [Header("Instrument Music")]
    public AudioClip instrumentMusic;
    public float musicSegmentDuration = 0.5f; // Duration of each music segment played

    private float lastAttack = 0f;
    private float currentMusicTime = 0f; // Track position in the music file

    public void ResetCooldown()
    {
        lastAttack = 0f;
    }

    public void Fire()
    {
        if (Time.time >= lastAttack)
        {
            Debug.Log("Successfully fired " + instrumentName);

            // Play instrument music segment
            if (AudioManager.Instance != null && instrumentMusic != null)
            {
                // Play the next segment of the instrument music
                AudioManager.Instance.PlayInstrumentMusic(instrumentMusic, currentMusicTime, musicSegmentDuration);

                // Advance the position in the music
                currentMusicTime += musicSegmentDuration;

                // Loop back to the beginning if we've reached the end
                if (currentMusicTime >= instrumentMusic.length)
                {
                    currentMusicTime = 0f;
                }
            }

            var projectile = Instantiate(projectileSpawn, projectilePosition.position, projectilePosition.rotation);
            var projectileData = projectile.GetComponent<Projectile>();
            projectileData.Initialize(gameObject);
            projectileData.damage = attackDamage;
            projectileData.speed = projectileSpeed;

            lastAttack = Time.time + attackCooldown;
        }
        else
        {
            Debug.LogWarning($"<color=yellow>{instrumentName} is on cooldown</color>");
            return;
        }


    }
}
