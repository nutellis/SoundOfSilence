using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed;

    public float lifetime = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
        else
        {
            lifetime -= Time.deltaTime;
        }

        transform.position += speed * Time.deltaTime * transform.forward;
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile collided with " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<Monster>(out var monster))
        {
            monster.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
