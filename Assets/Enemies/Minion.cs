using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2.5f;
    public float reachThreshold = 0.2f;
    public int deathCost;
    public int explosionDmg;

    int currentIndex = 0;

    public ActorType enemyType = ActorType.Melee;

    public Animator animator;

    public GameObject deathVFX;

    private ActorState state;

    public Action onDestroy;

    public void Initialize(Transform[] laneWaypoints)
    {
        waypoints = laneWaypoints;
    }

    private void Start()
    {
        state = GetComponent<ActorState>();
        animator.SetFloat("speed", 1 / moveSpeed);

        transform.forward = -Vector3.forward;

        Health health = GetComponent<Health>();
        if(health != null)
        {
            health.shouldDestroy += AnimateDeath;
        }

    }

    private void Update()
    {
        if (!state.isAttacking && !state.isDying)
        {
            if (waypoints == null || waypoints.Length == 0) return;
            Transform target = waypoints[currentIndex];
            Vector3 dir = (target.position - transform.position);
            dir.y = 0;
            if (dir.magnitude < reachThreshold)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    Explode();
                    return;
                }
                target = waypoints[currentIndex];
                dir = (target.position - transform.position);
            }

            Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;

            state.isWalking = true;

        }
        animator.SetBool("isAttacking", state.isAttacking);
        animator.SetBool("isWalking", state.isWalking);
    }

    void Explode()
    {
        Explosion();
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.GetComponent<Health>().TakeDamage(explosionDmg);
        }

        Die();
    }

    public void Die()
    {
        onDestroy?.Invoke();
        Destroy(gameObject);
    }

    //ty chatgpt
    void Explosion()
    {
        var vfx = Instantiate(deathVFX, transform.position, Quaternion.identity);
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps == null) ps = vfx.GetComponentInChildren<ParticleSystem>();

        if (ps != null)
        {
            ps.Play();
            var main = ps.main;
            float duration = main.duration;

            // use constantMax to be safe if startLifetime is a range
            float lifetime = main.startLifetime.constantMax;

            float life = duration + lifetime;
            Destroy(vfx, life/2 + 0.1f);
        }
        else
        {
            Destroy(vfx, 5f); // fallback
        }
    }

    void AnimateDeath()
    {
        AttackManager manager = FindFirstObjectByType<AttackManager>();
        if (manager != null)
        {
            manager.OnMinionDeath(deathCost);
        }
        state.isDying = true;
        animator.SetBool("isDying", state.isDying);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
    }
}
