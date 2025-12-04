using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float speed;

    public float lifetime = 20f;

    private GameObject owner;

    private Vector3 direction = Vector3.forward;


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

        transform.position += speed * Time.deltaTime * direction;
    }

    public void Initialize(GameObject parent)
    {
        owner = parent;

        //ugly but it works
        if (owner.tag == "Enemy")
        {
            direction = -direction;
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner)
        {
            return;
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        { 
            return;
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            Destroy(gameObject);
            return;
        } 
        else
        {
            Debug.Log("Projectile collided with " + collision.gameObject.name);
            if (collision.gameObject.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
