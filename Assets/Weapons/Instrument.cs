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
