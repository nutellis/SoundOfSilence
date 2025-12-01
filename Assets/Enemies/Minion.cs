using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2.5f;
    public float reachThreshold = 0.2f;

    int currentIndex = 0;

    public ActorType enemyType = ActorType.Melee;

    public Animator animator;

    private ActorState state;

    public Action OnDestroyed;

    public void Initialize(Transform[] laneWaypoints)
    {
        waypoints = laneWaypoints;
    }

    private void Start()
    {
        state = GetComponent<ActorState>();
        animator.SetFloat("speed", 1 / moveSpeed);
    }

    private void Update()
    {
        if (!state.isAttacking)
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
                    ReachEnd();
                    return;
                }
                target = waypoints[currentIndex];
                dir = (target.position - transform.position);
            }

            Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;
            transform.forward = Vector3.Slerp(transform.forward, dir.normalized, 0.2f);
        }

        animator.SetBool("isAttacking", state.isAttacking);
    }

    void ReachEnd()
    {
        // TODO: implement attack on player, damage, etc, then destroy
        DestroySelf();
    }

    public void TakeDamage(float dmg)
    {
        // TODO: monster health
        DestroySelf();
    }

    public void DestroySelf()
    {
        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
