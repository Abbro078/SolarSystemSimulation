using UnityEngine;

public class HomingMeteor : MonoBehaviour
{
    private GameObject target;
    private float speed;
    private float lifetime;
    private Rigidbody rb;
    public GameObject hitEffectPrefab; // Reference to the particle effect prefab

    public void Initialize(GameObject target, float speed, float lifetime)
    {
        this.target = target;
        this.speed = speed;
        this.lifetime = lifetime;

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("HomingMeteor requires a Rigidbody component.");
            Destroy(gameObject);
        }

        rb.useGravity = false; // Ensure gravity is disabled
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;

            // Optional: Add some rotation for visual effect
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Celestial"))
        {
            // Log the collision
            Debug.Log($"Meteor hit {collision.gameObject.name} at {collision.contacts[0].point}");

            // Create the hit effect
            if (hitEffectPrefab != null)
            {
                // Calculate the position and rotation for the hit effect
                Vector3 collisionPoint = collision.contacts[0].point;
                Quaternion hitEffectRotation = Quaternion.LookRotation(collision.contacts[0].normal);

                // Instantiate the hit effect at the collision point and parent it to the celestial body
                GameObject hitEffect = Instantiate(hitEffectPrefab, collisionPoint, hitEffectRotation, collision.transform);

                // Destroy the hit effect after 3 seconds
                Destroy(hitEffect, 1.5f);
            }

            // Destroy the meteor
            Destroy(gameObject);
        }
    }
}
