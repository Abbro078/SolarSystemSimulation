using UnityEngine;

public class MeteorShooter : MonoBehaviour
{
    [SerializeField] private GameObject[] meteorPrefabs; // Array of meteor prefabs
    [SerializeField] private float shootingSpeed = 200f;
    [SerializeField] private float meteorLifetime = 20f;
    [SerializeField] private float maxShootDistance = 1000f; // Maximum distance to check for planets

    private Camera _mainCamera;
    private System.Random _random = new System.Random();

    private void Start()
    {
        _mainCamera = Camera.main;
        // if (_mainCamera == null)
        // {
        //     Debug.LogError("Main Camera not found. Please ensure there is a camera tagged 'MainCamera' in the scene.");
        // }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            ShootMeteor();
        }
    }

    private void ShootMeteor()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxShootDistance))
        {
            GameObject target = hit.collider.CompareTag("Celestial") ? hit.collider.gameObject : null;
            LaunchMeteor(target, ray.direction);
        }
        else
        {
            Debug.Log("No target in range. Shooting in view direction.");
            LaunchMeteor(null, ray.direction);
        }
    }

    private void LaunchMeteor(GameObject target, Vector3 direction)
    {
        GameObject meteor = Instantiate(GetRandomMeteorPrefab(), transform.position, Quaternion.identity);
        Rigidbody meteorRb = meteor.GetComponent<Rigidbody>();

        if (meteorRb == null)
        {
            Debug.LogError("Meteor prefab must have a Rigidbody component!");
            Destroy(meteor);
            return;
        }

        if (target != null)
        {
            HomingMeteor homingMeteor = meteor.AddComponent<HomingMeteor>();
            homingMeteor.Initialize(target, shootingSpeed, meteorLifetime);
        }
        else
        {
            meteorRb.AddForce(direction * shootingSpeed, ForceMode.Impulse);
            meteorRb.useGravity = false; // Typically false in space environments
        }

        Destroy(meteor, meteorLifetime);
    }

    private GameObject GetRandomMeteorPrefab()
    {
        if (meteorPrefabs.Length == 0)
        {
            Debug.LogError("No meteor prefabs assigned.");
            return null;
        }
        int index = _random.Next(meteorPrefabs.Length);
        return meteorPrefabs[index];
    }
}
