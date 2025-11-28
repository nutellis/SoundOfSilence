using UnityEngine;

public class Peojectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 1;

    private void Start()
    {
        // destroy bullet after X seconds to avoid clutter
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // move forward constantly
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Example: if it hits the player
        if (other.CompareTag("Player"))
        {
            // Apply damage if you have a player health script
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Destroy(gameObject);
        }

        // Destroy bullet on hitting any collider (optional)
        //if (!other.CompareTag("Enemy"))
        //{
        //    Destroy(gameObject);
        //}
    }
}