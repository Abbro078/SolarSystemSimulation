using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] asteroidPrefabs;
    [SerializeField] private int cubeDensity;
    [SerializeField] private int seed;
    [SerializeField] private float innerRadius;
    [SerializeField] private float outerRadius;
    [SerializeField] private float height;
    [SerializeField] private bool rotatingClockwise;

    [Header("Asteroid Settings")]
    [SerializeField] private float minOrbitSpeed;
    [SerializeField] private float maxOrbitSpeed;
    [SerializeField] private float minRotationSpeed;
    [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;

    private Vector3 _localPosition;
    private Vector3 _worldOffset;
    private Vector3 _worldPosition;
    private float _randomRadius;
    private float _randomRadian;
    private float _x;
    private float _y;
    private float _z;

    private void Start()
    {
        Random.InitState(seed);

        // Find the Saturn GameObject
        GameObject saturn = GameObject.Find("Saturn");
        if (saturn == null)
        {
            Debug.LogError("Saturn GameObject not found!");
            return;
        }

        // Create or find the Ring of Saturn GameObject as a child of Saturn
        GameObject ringOfSaturn = GameObject.Find("Ring of Saturn");
        if (ringOfSaturn == null)
        {
            ringOfSaturn = new GameObject("Ring of Saturn");
            ringOfSaturn.transform.SetParent(saturn.transform);
            ringOfSaturn.transform.localPosition = Vector3.zero;
        }

        for (int i = 0; i < cubeDensity; i++)
        {
            do
            {
                _randomRadius = Random.Range(innerRadius, outerRadius);
                _randomRadian = Random.Range(0, (2 * Mathf.PI));

                _y = Random.Range(-(height / 2), (height / 2));
                _x = _randomRadius * Mathf.Cos(_randomRadian);
                _z = _randomRadius * Mathf.Sin(_randomRadian);
            }
            while (float.IsNaN(_z) && float.IsNaN(_x));

            _localPosition = new Vector3(_x, _y, _z);
            _worldOffset = ringOfSaturn.transform.rotation * _localPosition;
            _worldPosition = ringOfSaturn.transform.position + _worldOffset;

            // Randomly select one of the asteroid prefabs
            GameObject selectedPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];

            GameObject _asteroid = Instantiate(selectedPrefab, _worldPosition, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            _asteroid.AddComponent<BeltObject>().SetupBeltObject(Random.Range(minOrbitSpeed, maxOrbitSpeed), Random.Range(minRotationSpeed, maxRotationSpeed), saturn, rotatingClockwise);

            // Randomize size
            float randomSize = Random.Range(minSize, maxSize);
            _asteroid.transform.localScale = new Vector3(randomSize, randomSize, randomSize);

            // Set the parent to Ring of Saturn
            _asteroid.transform.SetParent(ringOfSaturn.transform);
        }
    }
}
