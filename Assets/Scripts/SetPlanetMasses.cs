using UnityEngine;

public class SetPlanetMasses : MonoBehaviour
{
    // Reference to all celestial bodies
    public GameObject[] celestialBodies;

    void Start()
    {
        // Set the mass of each celestial body
        SetMasses();
    }

    void SetMasses()
    {
        // Masses in kg
        float[] masses = {
            4.126e13f, // Mercury
            6.084e14f, // Venus
            7.465e14f, // Earth
            8.025e13f, // Mars
            2.373e17f, // Jupiter
            7.105e16f, // Saturn
            1.085e16f, // Uranus
            1.280e16f, // Neptune
            2.483e20f  // Sun
        };

        for (int i = 0; i < celestialBodies.Length; i++)
        {
            Rigidbody rb = celestialBodies[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = masses[i];
            }
        }
    }
}