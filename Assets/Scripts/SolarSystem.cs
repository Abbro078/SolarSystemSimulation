using System;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] private float universeGravity = 100f;
    
    // Atmospheric constants
    [SerializeField] private float seaLevelDensity = 1.225f; // kg/m^3 at sea level
    [SerializeField] private float scaleHeight = 8400f; // Scale height in meters (for exponential decay)
    

    private GameObject[] _celestialBodies;
    private Rigidbody[] _celestialRigidbodies;
    

    // Update the radii to match your celestial body sizes
    private Dictionary<string, float> radii = new Dictionary<string, float>()
    {
        {"Sun", 696.340f},
        {"Mercury", 2.440f},
        {"Venus", 6.052f},
        {"Earth", 6.371f},
        {"Mars", 3.390f},
        {"Jupiter", 69.911f},
        {"Saturn", 58.232f},
        {"Uranus", 25.362f},
        {"Neptune", 24.622f},
        {"Moon", 1.7f}
    };

    private Dictionary<string, float> rotationalPeriods = new Dictionary<string, float>()
    {
        {"Sun", 25.0f * 24 * 60 * 60}, // Example: Sun rotates once every 25 days
        {"Mercury", 58.6f * 24 * 60 * 60}, // Mercury rotates once every 58.6 days
        {"Venus", -243.0f * 24 * 60 * 60}, // Venus rotates once every 243 days
        {"Earth", 24.0f * 60 * 60}, // Earth rotates once every 24 hours
        {"Mars", 24.6f * 60 * 60}, // Mars rotates once every 24.6 hours
        {"Jupiter", 9.9f * 60 * 60}, // Jupiter rotates once every 9.9 hours
        {"Saturn", 10.7f * 60 * 60}, // Saturn rotates once every 10.7 hours
        {"Uranus", -17.2f * 60 * 60}, // Uranus rotates once every 17.2 hours
        {"Neptune", 16.1f * 60 * 60}, // Neptune rotates once every 16.1 hours
        {"Moon", 27.3f * 24 * 60 * 60} // Moon rotates once every 27.3 days (synchronous rotation with Earth)
    };
    
    private Dictionary<string, float> semiMajorAxes = new Dictionary<string, float>()
    {
        {"Mercury", 638.0f}, // Example semi-major axis
        {"Venus", 1076.0f},
        {"Earth", 1510.0f},
        {"Mars", 2488.0f},
        {"Jupiter", 7559.0f},
        {"Saturn", 14878.0f},
        {"Uranus", 29546.0f},
        {"Neptune", 44755.0f},
        {"Moon", 1522.05f}
    };

    private Dictionary<string, float> planetaryMasses = new Dictionary<string, float>()
    {
        {"Mercury", 0.055f},
        {"Venus", 0.815f},
        {"Earth", 15.0f},
        {"Mars", 0.107f},
        {"Jupiter", 317.8f},
        {"Saturn", 95.16f},
        {"Uranus", 14.54f},
        {"Neptune", 17.15f},
        {"Moon", 0.01f}
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

                float gravitationalForce = CalculateGravitationalForce(distance, massA, massB, bodyA.gameObject.name, bodyB.gameObject.name);
                Vector3 force = direction.normalized * gravitationalForce;

                bodyA.AddForce(force);
                bodyB.AddForce(-force); // Applying Newton's third law
            }
        }
    }

    float CalculateGravitationalForce(float distance, float massA, float massB, string nameA, string nameB)
    {
        float radiusA = radii.ContainsKey(nameA) ? radii[nameA] : 0f;
        float radiusB = radii.ContainsKey(nameB) ? radii[nameB] : 0f;

        float effectiveDistance = Mathf.Max(distance, 0.1f); // Avoid division by zero

        float gravityFactorA = radiusA > 0 ? Mathf.Clamp01(effectiveDistance / radiusA) : 1f;
        float gravityFactorB = radiusB > 0 ? Mathf.Clamp01(effectiveDistance / radiusB) : 1f;

        // Calculate atmospheric density effect
        float altitudeA = Mathf.Max(0, effectiveDistance - radiusA);
        float atmosphericDensityA = seaLevelDensity * Mathf.Exp(-altitudeA / scaleHeight);

        float altitudeB = Mathf.Max(0, effectiveDistance - radiusB);
        float atmosphericDensityB = seaLevelDensity * Mathf.Exp(-altitudeB / scaleHeight);

        // Apply density adjustment to the gravitational force
        float densityFactorA = 1f + (0.1f * atmosphericDensityA); // Adjust as needed
        float densityFactorB = 1f + (0.1f * atmosphericDensityB); // Adjust as needed

        if (effectiveDistance <= radiusA || effectiveDistance <= radiusB)
        {
            // Gravity decreases as we move from the center to the surface
            return universeGravity * (massA * massB) / (effectiveDistance * effectiveDistance) * gravityFactorA * gravityFactorB * densityFactorA * densityFactorB;
        }
        else
        {
            return universeGravity * (massA * massB) / (effectiveDistance * effectiveDistance) * densityFactorA * densityFactorB;
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
    
    
    // private void CalculateAndApplyRotation()
    // {
    //     foreach (var kvp in semiMajorAxes)
    //     {
    //         string planet = kvp.Key;
    //         float semiMajorAxis = kvp.Value;
    //         float mass = planetaryMasses[planet];
    //
    //         // Calculate the rotational period
    //         float period = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(semiMajorAxis, 3) / (100000000f * mass));
    //
    //         Debug.Log($"{planet} has a rotational period of {period} seconds.");
    //
    //         // Apply the rotational period to your celestial objects
    //         ApplyRotation(planet, period);
    //     }
    // }
    //
    // private void ApplyRotation(string planetName, float period)
    // {
    //     float rotationSpeed = 2 * Mathf.PI / period;
    //
    //     foreach (Rigidbody body in _celestialRigidbodies)
    //     {
    //         if (body.gameObject.name == planetName)
    //         {
    //             body.angularVelocity = Vector3.up * rotationSpeed;
    //             break;
    //         }
    //     }
    // }

    // You need to define _celestialRigidbodies somewhere in your class
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