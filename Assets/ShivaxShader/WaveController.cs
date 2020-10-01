using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float speed = 1;
    [SerializeField] private float radius = 1;

    [Range(1, shaderArraySizeLimit)]
    [SerializeField] private int maximumWaves = 1;

    [SerializeField] private List<Wave> waves;

    // Value fixed and synchronized with the maximum size of an array in the shader
    private const int shaderArraySizeLimit = 100;

    private void Start()
    {
        waves = new List<Wave>();

        // Set the maximum size of the arrays used by the shader 
        material.SetVectorArray("_Origins", new Vector4[shaderArraySizeLimit]);
    }

    private void Update()
    {
        material.SetFloat("_MaxRadius", radius);
        material.SetInt("_WavesCount", waves.Count);
      
        HandleWaves();

        UpdateShader();
    }

    public void CreateWave(Vector3 worldPos)
    {
        if (waves.Count >= maximumWaves) return;

        var wave = new Wave(worldPos, radius);

        waves.Insert(0, wave);
    }

    /// <summary>
    /// Updates waves progress and remove olders ones
    /// </summary>
    private void HandleWaves()
    {
        // Calculate speeds progression
        for (int i = 0; i < waves.Count; i++)
        {
            var wave = waves[i];
            wave.UpdateDistance(speed);
        }

        waves = waves.Where(wave => wave.Distance < Wave.MAXIMUM_DISTANCE).ToList();
    }

    private void UpdateShader()
    {
        if (waves.Count > 0)
        {
            var originsArray = ExtractOrigins(waves);

            material.SetVectorArray("_Origins", originsArray);
        }
    }

    /// <summary>
    /// Extract waves origins from a list of waves
    /// </summary>
    /// <param name="waves"></param>
    /// <returns></returns>
    private static Vector4[] ExtractOrigins(List<Wave> waves)
    {
        var origins = new List<Vector4>();

        foreach (var wave in waves)
        {
            var origin = wave.Origin;

            origins.Add(new Vector4(origin.x, origin.y, origin.z, wave.Distance));
        }

        return origins.ToArray();
    }
}