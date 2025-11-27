using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health = 100;
    public int damage = 10;
    public float speed = 5.0f;
    public float attackSpeed = 2.0f;

    public Animator animator;


    private Transform player;

    private bool isAttacking = false;
    private bool isWalking = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null) return;

        Vector3 dir = (player.position - transform.position).normalized;

        transform.forward = dir;

        InvokeRepeating(nameof(Attack), 2.0f, attackSpeed);
    }

    private void Update()
    {
        if (player == null) return;

        //  transform.position += transform.forward * speed * Time.deltaTime;

        //animator.SetBool("isWalking", isWalking);
        animator.SetBool("isAttacking", isAttacking);

    }

    public void Attack()
    {
        //throw projectile or deal damage to player
        Debug.Log("Enemy attacks player for " + damage + " damage!");

        isWalking = false;
        isAttacking = true;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void OnSpellCastingFinished()
    {
        isAttacking = false;
        isWalking = true;
    }


}
