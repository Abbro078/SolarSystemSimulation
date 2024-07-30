using UnityEngine;

public class MeteorShooter : MonoBehaviour
{
    public GameObject meteorPrefab;
    public float shootingSpeed = 200f;
    public float meteorLifetime = 20f;
    public float maxShootDistance = 1000f; // Maximum distance to check for planets

    private Camera mainCamera;

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
                LaunchMeteorAtTarget(targetPlanet);
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

    void LaunchMeteorAtTarget(GameObject target)
    {
        GameObject meteor = Instantiate(meteorPrefab, transform.position, Quaternion.identity);
        Rigidbody meteorRb = meteor.GetComponent<Rigidbody>();

        if (meteorRb != null)
        {
            Vector3 targetPosition = PredictTargetPosition(target, meteor.transform.position);
            Vector3 shootDirection = (targetPosition - meteor.transform.position).normalized;

            meteorRb.AddForce(shootDirection * shootingSpeed, ForceMode.Impulse);
            meteorRb.useGravity = false;

            Destroy(meteor, meteorLifetime);
        }
        else
        {
            Debug.LogError("Meteor prefab must have a Rigidbody component!");
        }
    }

    void LaunchMeteorInDirection(Vector3 direction)
    {
        GameObject meteor = Instantiate(meteorPrefab, transform.position, Quaternion.identity);
        Rigidbody meteorRb = meteor.GetComponent<Rigidbody>();

        if (meteorRb != null)
        {
            meteorRb.AddForce(direction * shootingSpeed, ForceMode.Impulse);
            meteorRb.useGravity = true;

            Destroy(meteor, meteorLifetime);
        }
        else
        {
            Debug.LogError("Meteor prefab must have a Rigidbody component!");
        }
    }

    Vector3 PredictTargetPosition(GameObject target, Vector3 shooterPosition)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb == null) return target.transform.position;

        float distance = Vector3.Distance(shooterPosition, target.transform.position);
        float timeToTarget = distance / shootingSpeed;

        return target.transform.position + (targetRb.velocity * timeToTarget);
    }
}