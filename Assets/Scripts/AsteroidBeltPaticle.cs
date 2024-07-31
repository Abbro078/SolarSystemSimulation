using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AsteroidBeltParticleSystem : MonoBehaviour
{
    [Header("Asteroid Belt Settings")]
    public Mesh asteroidMesh;
    public Material asteroidMaterial;
    public int asteroidCount = 1000;
    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    public float innerRadius = 50f;
    public float outerRadius = 100f;
    public float height = 10f;

    private void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();

        // Main Module
        main.maxParticles = asteroidCount;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0;
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);

        // Emission Module
        emission.rateOverTime = 0;
        emission.rateOverDistance = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, asteroidCount) });

        // Shape Module
        shape.shapeType = ParticleSystemShapeType.Donut;
        shape.radius = (innerRadius + outerRadius) / 2;
        shape.donutRadius = (outerRadius - innerRadius) / 2;
        shape.arcMode = ParticleSystemShapeMultiModeValue.Loop;
        shape.arcSpread = 1f;

        // Renderer Module
        renderer.renderMode = ParticleSystemRenderMode.Mesh;
        renderer.mesh = asteroidMesh;
        renderer.material = asteroidMaterial;
        
        // Enable shadows
        renderer.castShadows = true;
        renderer.receiveShadows = true;
    }
}
