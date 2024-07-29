using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light pointLight; // The point light source
    public float maxIntensity = 1.0f; // Maximum intensity at the light source
    public float maxDistance = 10.0f; // Maximum distance where the light is still effective

    void Update()
    {
        // Iterate through all objects in the scene that should be affected by the light
        foreach (var obj in FindObjectsOfType<Renderer>())
        {
            // Calculate the distance from the light source to the object
            float distance = Vector3.Distance(pointLight.transform.position, obj.transform.position);

            // Calculate the intensity based on the distance
            float intensity = Mathf.Clamp01(1 - (distance / maxDistance)) * maxIntensity;

            // Apply the intensity to the object's material
            // obj.material.SetFloat("_LightIntensity", intensity);
        }
    }
}
