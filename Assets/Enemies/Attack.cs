using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    private float nextAttackTime;

    private ActorState state;
    private ActorType actorType;

    private Transform player;

    public Transform projectilePosition;
    public GameObject projectileSpawn;
    public int projectileSpeed = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GetComponent<ActorState>();
        actorType = GetComponent<Minion>().enemyType;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || state.isAttacking) return;

        if(Time.time >= nextAttackTime)
        {
            if (actorType == ActorType.Melee)
            {
                float dist = Vector3.Distance(transform.position, player.position);
                if (dist <= attackRange)
                {
                    state.isWalking = false;
                    state.isAttacking = true;
                }
            }
            else if (actorType == ActorType.Ranged)
            {
                state.isWalking = false;
                state.isAttacking = true;
            }
        }
    }

    public void CastSpell()
    {
        Debug.Log("Enemy attacks player for " + attackDamage + " damage!");
        // Throw projectile towards player

        // Play fire crackle sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFireCrackle();
        }

        var projectile = Instantiate(projectileSpawn, projectilePosition.position, projectilePosition.rotation);
        projectile.GameObject().GetComponent<Projectile>().Initialize(gameObject);

        var projectileData = projectile.GetComponent<Projectile>();
        projectileData.damage = attackDamage;
        projectileData.speed = projectileSpeed;


        Debug.Log("Ranged enemy throws a projectile at the player!");
    }

    //im lazy :)
    public void RangedAttackFinished()
    {
        state.isAttacking = false;
        state.isWalking = true;
        nextAttackTime = Time.time + attackCooldown;
    }

    public void PerformMeleeAttack()
    {

        Debug.Log("Enemy attacks player for " + attackDamage + " damage!");

        player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);

        state.isAttacking = false;
        state.isWalking = true;
        nextAttackTime = Time.time + attackCooldown;
    }


}
