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
       // Debug.Log("CoolDown: " + (nextAttackTime - Time.time));
    }

    public void PerformAttack()
    {

        Debug.Log("Enemy attacks player for " + attackDamage + " damage!");

        if (actorType == ActorType.Ranged)
        {
            // Throw projectile towards player
            Debug.Log("Ranged enemy throws a projectile at the player!");

            state.isAttacking = false;
            state.isWalking = true;
        }

        nextAttackTime = Time.time + attackCooldown;
    }


}
