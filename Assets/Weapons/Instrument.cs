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

    [Header("Instrument Sounds")]
    public AudioClip[] instrumentSounds;
    [Range(0f, 1f)] public float instrumentVolume = 0.8f;

    private float lastAttack = 0f;

    public void ResetCooldown()
    {
        lastAttack = 0f;
    }

    public void Fire()
    {
        if (Time.time >= lastAttack)
        {
            Debug.Log("Successfully fired " + instrumentName);

            // Play random instrument sound effect via AudioManager
            if (AudioManager.Instance != null && instrumentSounds != null && instrumentSounds.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, instrumentSounds.Length);
                AudioManager.Instance.PlayInstrumentSound(instrumentSounds[randomIndex], instrumentVolume);
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
