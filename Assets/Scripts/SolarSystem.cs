using System;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] private float universeGravity = 100f;

    private GameObject[] _celestialBodies;
    private Rigidbody[] _celestialRigidbodies;

    private Dictionary<string, float> rotationalPeriods = new Dictionary<string, float>()
    {
        {"Sun", 25.0f * 24 * 60 * 60}, // Example: Sun rotates once every 25 days
        {"Mercury", 58.6f * 24 * 60 * 60}, // Mercury rotates once every 58.6 days
        {"Venus", 243.0f * 24 * 60 * 60}, // Venus rotates once every 243 days
        {"Earth", 24.0f * 60 * 60}, // Earth rotates once every 24 hours
        {"Mars", 24.6f * 60 * 60}, // Mars rotates once every 24.6 hours
        {"Jupiter", 9.9f * 60 * 60}, // Jupiter rotates once every 9.9 hours
        {"Saturn", 10.7f * 60 * 60}, // Saturn rotates once every 10.7 hours
        {"Uranus", 17.2f * 60 * 60}, // Uranus rotates once every 17.2 hours
        {"Neptune", 16.1f * 60 * 60}, // Neptune rotates once every 16.1 hours
        {"Moon", 27.3f * 24 * 60 * 60} // Moon rotates once every 27.3 days (synchronous rotation with Earth)
    };

    void Start()
    {
        _celestialBodies = GameObject.FindGameObjectsWithTag("Celestial");
        _celestialRigidbodies = new Rigidbody[_celestialBodies.Length];

        for (int i = 0; i < _celestialBodies.Length; i++)
        {
            _celestialRigidbodies[i] = _celestialBodies[i].GetComponent<Rigidbody>();
        }

        InitializeVelocity();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        ApplySelfRotation();
    }

    void ApplyGravity()
    {
        for (int i = 0; i < _celestialBodies.Length; i++)
        {
            Rigidbody bodyA = _celestialRigidbodies[i];
            Vector3 positionA = bodyA.position;
            float massA = bodyA.mass;

            for (int j = i + 1; j < _celestialBodies.Length; j++)
            {
                Rigidbody bodyB = _celestialRigidbodies[j];
                Vector3 positionB = bodyB.position;
                float massB = bodyB.mass;

                Vector3 direction = positionB - positionA;
                float distance = direction.magnitude;
                Vector3 force = direction.normalized * (universeGravity * (massA * massB) / (distance * distance));

                bodyA.AddForce(force);
                bodyB.AddForce(-force); // Applying Newton's third law
            }
        }
    }

    void InitializeVelocity()
    {
        for (int i = 0; i < _celestialBodies.Length; i++)
        {
            Rigidbody bodyA = _celestialRigidbodies[i];
            Vector3 positionA = bodyA.position;

            for (int j = 0; j < _celestialBodies.Length; j++)
            {
                if (i == j) continue;

                Rigidbody bodyB = _celestialRigidbodies[j];
                Vector3 positionB = bodyB.position;
                float massB = bodyB.mass;

                Vector3 direction = positionB - positionA;
                float distance = direction.magnitude;
                Vector3 velocity = Vector3.Cross(direction.normalized, Vector3.up) * Mathf.Sqrt((universeGravity * massB) / distance);

                bodyA.velocity += velocity;
            }
        }
    }

    void ApplySelfRotation()
    {
        for (int i = 0; i < _celestialRigidbodies.Length; i++)
        {
            Rigidbody body = _celestialRigidbodies[i];
            string bodyName = body.gameObject.name;

            if (rotationalPeriods.ContainsKey(bodyName))
            {
                float rotationPeriod = rotationalPeriods[bodyName];
                float rotationSpeed = 2 * Mathf.PI / rotationPeriod; // radians per second

                // Apply angular velocity for self-rotation around the up axis (y-axis)
                body.angularVelocity = Vector3.up * rotationSpeed;
            }
            else
            {
                Debug.LogWarning($"Rotational period not defined for {bodyName}");
            }
        }
    }
}
