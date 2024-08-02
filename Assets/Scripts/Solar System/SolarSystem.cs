using System;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] private float universeGravity = 100f;
    [SerializeField] private float seaLevelDensity = 1.225f; // kg/m^3 at sea level
    [SerializeField] private float scaleHeight = 8400f; // Scale height in meters

    private GameObject[] _celestialBodies;
    private Rigidbody[] _celestialRigidbodies;

    private void Start()
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

    private void ApplyGravity()
    {
        for (int i = 0; i < _celestialBodies.Length; i++)
        {
            Rigidbody bodyA = _celestialRigidbodies[i];

            for (int j = i + 1; j < _celestialBodies.Length; j++)
            {
                Rigidbody bodyB = _celestialRigidbodies[j];
                Vector3 direction = bodyB.position - bodyA.position;
                float distance = direction.magnitude;
                float gravitationalForce = CalculateGravitationalForce(distance, bodyA.mass, bodyB.mass, bodyA, bodyB);
                Vector3 force = direction.normalized * gravitationalForce;

                bodyA.AddForce(force);
                bodyB.AddForce(-force);
            }
        }
    }

    private float CalculateGravitationalForce(float distance, float massA, float massB, Rigidbody bodyA, Rigidbody bodyB)
    {
        float radiusA = bodyA.transform.localScale.x / 2;
        float radiusB = bodyB.transform.localScale.x / 2;

        float effectiveDistance = Mathf.Max(distance, 0.1f);

        float gravityFactorA = radiusA > 0 ? Mathf.Clamp01(effectiveDistance / radiusA) : 1f;
        float gravityFactorB = radiusB > 0 ? Mathf.Clamp01(effectiveDistance / radiusB) : 1f;

        float altitudeA = Mathf.Max(0, effectiveDistance - radiusA);
        float atmosphericDensityA = seaLevelDensity * Mathf.Exp(-altitudeA / scaleHeight);

        float altitudeB = Mathf.Max(0, effectiveDistance - radiusB);
        float atmosphericDensityB = seaLevelDensity * Mathf.Exp(-altitudeB / scaleHeight);

        float densityFactorA = 1f + (0.1f * atmosphericDensityA);
        float densityFactorB = 1f + (0.1f * atmosphericDensityB);

        float gravity = universeGravity * (massA * massB) / (effectiveDistance * effectiveDistance);
        if (effectiveDistance <= radiusA || effectiveDistance <= radiusB)
        {
            return gravity * gravityFactorA * gravityFactorB * densityFactorA * densityFactorB;
        }
        return gravity * densityFactorA * densityFactorB;
    }

    private void InitializeVelocity()
    {
        for (int i = 0; i < _celestialBodies.Length; i++)
        {
            Rigidbody bodyA = _celestialRigidbodies[i];

            for (int j = 0; j < _celestialBodies.Length; j++)
            {
                if (i == j) continue;

                Rigidbody bodyB = _celestialRigidbodies[j];
                Vector3 direction = bodyB.position - bodyA.position;
                float distance = direction.magnitude;
                Vector3 velocity = Vector3.Cross(direction.normalized, Vector3.up) * Mathf.Sqrt((universeGravity * bodyB.mass) / distance);

                bodyA.velocity += velocity;
            }
        }
    }

    private void ApplySelfRotation()
    {
        foreach (Rigidbody body in _celestialRigidbodies)
        {
            string bodyName = body.gameObject.name;
            if (CelestialBodyData.RotationalPeriods.ContainsKey(bodyName))
            {
                float rotationSpeed = 2 * Mathf.PI / CelestialBodyData.RotationalPeriods[bodyName];
                body.angularVelocity = Vector3.up * rotationSpeed;
            }
            else
            {
                //Debug.LogWarning($"Rotational period not defined for {bodyName}");
            }
        }
    }
}
