using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Instrument : MonoBehaviour
{
    public string instrumentName;
    public InputActionReference triggerAction;
    public GameObject projectilePrefab;
    public float damage;
    public int projectileSpeed;
    public float attackRate;

    private float lastAttack = 0f;

    public void ResetCooldown()
    {
        lastAttack = 0f;
    }

    public void Fire(Transform spawn)
    {
        // Implementation for firing the instrument
        Debug.Log(Time.time + ": Attempting to fire " + instrumentName);
        Debug.Log("Last attack time: " + lastAttack);
        var timeLeft = lastAttack + attackRate - Time.time;
        if (timeLeft > 0f)
        {
            Debug.Log("Cannot fire " + instrumentName + ". Cooldown remaining: " + timeLeft);
            return;
        }
        Debug.Log("Successfully fired " + instrumentName);
        lastAttack = Time.time;
        var projectile = Instantiate(projectilePrefab, spawn.position, spawn.rotation);
        var projectileData = projectile.GetComponent<Projectile>();
        projectileData.damage = damage;
        projectileData.speed = projectileSpeed;
    }
}
