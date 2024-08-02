using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject cubePrefab;
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

        for (int i = 0; i < cubeDensity; i++)
        {
            do
            {
                _randomRadius = Random.Range(innerRadius, outerRadius);
                _randomRadian = Random.Range(0, (2 * Mathf.PI));

                _y = Random.Range(-(height /2), (height / 2));
                _x = _randomRadius * Mathf.Cos(_randomRadian);
                _z = _randomRadius * Mathf.Sin(_randomRadian);
            }
            while (float.IsNaN(_z) && float.IsNaN(_x));

            _localPosition = new Vector3(_x, _y, _z);
            _worldOffset = transform.rotation * _localPosition;
            _worldPosition = transform.position + _worldOffset;

            GameObject _asteroid = Instantiate(cubePrefab, _worldPosition, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            _asteroid.AddComponent<BeltObject>().SetupBeltObject(Random.Range(minOrbitSpeed, maxOrbitSpeed), Random.Range(minRotationSpeed, maxRotationSpeed), gameObject, rotatingClockwise);

            // Randomize size
            float randomSize = Random.Range(minSize, maxSize);
            _asteroid.transform.localScale = new Vector3(randomSize, randomSize, randomSize);

            _asteroid.transform.SetParent(transform);
        }
    }
}
