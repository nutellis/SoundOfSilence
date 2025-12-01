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
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //get health component and apply damage
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Destroy(gameObject);
        }

        // Destroy bullet on hitting any collider
        //if (!other.CompareTag("Enemy"))
        //{
        //    Destroy(gameObject);
        //}
    }
}