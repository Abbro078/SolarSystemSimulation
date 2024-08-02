using UnityEngine;

public class HomingMeteor : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab; 

    private GameObject _target;
    private float _speed;
    private float _lifetime;
    private Rigidbody _rb;

    public void Initialize(GameObject target, float speed, float lifetime)
    {
        _target = target;
        _speed = speed;
        _lifetime = lifetime;

        _rb = GetComponent<Rigidbody>();

        if (_rb == null)
        {
            Debug.LogError("HomingMeteor requires a Rigidbody component.");
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, _lifetime);
    }

    private void FixedUpdate()
    {
        if (_target == null) return;

        Vector3 direction = (_target.transform.position - transform.position).normalized;
        _rb.velocity = direction * _speed;

        // Update rotation to match velocity
        if (_rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_rb.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Celestial"))
        {
            HandleCollision(collision);
        }
    }

    private void HandleCollision(Collision collision)
    {
        Debug.Log($"Meteor hit {collision.gameObject.name} at {collision.contacts[0].point}");

        if (hitEffectPrefab != null)
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            Quaternion hitEffectRotation = Quaternion.LookRotation(collision.contacts[0].normal);

            GameObject hitEffect = Instantiate(hitEffectPrefab, collisionPoint, hitEffectRotation, collision.transform);
            Destroy(hitEffect, 1.5f);
        }

        Destroy(gameObject);
    }
}