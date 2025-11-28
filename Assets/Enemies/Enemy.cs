using UnityEngine;


public class Enemy : MonoBehaviour
{
    public float speed = 5.0f;
    public ActorType enemyType = ActorType.Melee;

    public Animator animator;

    private ActorState state;
    private Transform player;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null) return;

        state = GetComponent<ActorState>();

        animator.SetFloat("speed", 1/speed);

        Vector3 dir = (player.position - transform.position).normalized;

        transform.forward = dir;


    }

    private void Update()
    {
        if (player == null) return;

        if (!state.isAttacking)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        animator.SetBool("isAttacking", state.isAttacking);

    }

}
