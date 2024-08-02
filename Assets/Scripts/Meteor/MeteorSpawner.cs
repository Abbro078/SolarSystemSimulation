using UnityEngine;

public class MeteorShooter : MonoBehaviour
{
    public GameObject[] meteorPrefabs; // Array of meteor prefabs
    public float shootingSpeed = 200f;
    public float meteorLifetime = 20f;
    public float maxShootDistance = 1000f; // Maximum distance to check for planets

    private Camera mainCamera;
    private System.Random random = new System.Random();

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            ShootMeteor();
        }
    }

    void ShootMeteor()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxShootDistance))
        {
            // Check if we hit a celestial body
            if (hit.collider.CompareTag("Celestial"))
            {
                GameObject targetPlanet = hit.collider.gameObject;
                LaunchHomingMeteor(targetPlanet);
            }
            else
            {
                Debug.Log("No planet targeted. Shooting in view direction.");
                LaunchMeteorInDirection(ray.direction);
            }
        }
        else
        {
            Debug.Log("No target in range. Shooting in view direction.");
            LaunchMeteorInDirection(ray.direction);
        }
    }

    void LaunchHomingMeteor(GameObject target)
    {
        GameObject meteor = Instantiate(GetRandomMeteorPrefab(), transform.position, Quaternion.identity);
        HomingMeteor homingMeteor = meteor.AddComponent<HomingMeteor>();

        if (homingMeteor != null)
        {
            homingMeteor.Initialize(target, shootingSpeed, meteorLifetime);
        }
        else
        {
            Debug.LogError("Failed to add HomingMeteor component to the meteor!");
        }
    }

    void LaunchMeteorInDirection(Vector3 direction)
    {
        GameObject meteor = Instantiate(GetRandomMeteorPrefab(), transform.position, Quaternion.identity);
        Rigidbody meteorRb = meteor.GetComponent<Rigidbody>();

        if (meteorRb != null)
        {
            meteorRb.AddForce(direction * shootingSpeed, ForceMode.Impulse);
            meteorRb.useGravity = false; // Use gravity if needed, but for space, it's typically false

            Destroy(meteor, meteorLifetime);
        }
        else
        {
            Debug.LogError("Meteor prefab must have a Rigidbody component!");
        }
    }

    GameObject GetRandomMeteorPrefab()
    {
        int index = random.Next(meteorPrefabs.Length);
        return meteorPrefabs[index];
    }
}
