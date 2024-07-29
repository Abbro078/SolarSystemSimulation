using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] private Transform orbitCenter; // The object to orbit around (e.g., the Sun)
    [SerializeField] private float orbitSpeed; // Speed of the orbit in degrees/second
    [SerializeField] private float rotationSpeed; // Speed of the planet's own rotation in degrees/second

    private float orbitRadius; // Radius of the orbit
    private float angle; // Current angle in the orbit

    void Start()
    {
        // Initialize the orbitRadius
        orbitRadius = Vector3.Distance(this.gameObject.transform.position, orbitCenter.position);
    }

    void Update()
    {
        // Calculate the new position
        angle += orbitSpeed * Time.deltaTime;
        float radians = angle * Mathf.Deg2Rad;
        float x = orbitCenter.position.x + Mathf.Cos(angle) * orbitRadius;
        float z = orbitCenter.position.z + Mathf.Sin(angle) * orbitRadius;

        // Update the position of the planet
        transform.position = new Vector3(x, transform.position.y, z);
        
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

    }
}
