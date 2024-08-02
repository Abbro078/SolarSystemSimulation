using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private Light pointLight; 
    [SerializeField] private float maxIntensity = 1.0f; 
    [SerializeField] private float maxDistance = 10.0f; 

    private List<Renderer> _renderers = new List<Renderer>();

    private void Start()
    {
        _renderers.AddRange(FindObjectsOfType<Renderer>());
    }

    private void Update()
    {
        UpdateLightIntensity();
    }

    private void UpdateLightIntensity()
    {
        foreach (var renderer in _renderers)
        {
            float distance = Vector3.Distance(pointLight.transform.position, renderer.transform.position);

            float intensity = Mathf.Clamp01(1 - (distance / maxDistance)) * maxIntensity;

            
            Light light = renderer.GetComponent<Light>();
            if (light != null)
            {
                light.intensity = intensity;
            }
        }
    }
}