using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private Material material;

    [Range(1, shaderArraySizeLimit)]
    [SerializeField] private int maximumWaves = 1;

    [SerializeField] private List<Wave> waves;

    // Value fixed and synchronized with the maximum size of an array in the shader
    private const int shaderArraySizeLimit = 100;

    private static Vector4[] EMPTY_VECTOR_ARRAY = new Vector4[shaderArraySizeLimit];
    private static float[] EMPTY_FLOAT_ARRAY = new float[shaderArraySizeLimit];

    private void Start()
    {
        waves = new List<Wave>();
    }

    private void Update()
    {
        material.SetInt("_WavesCount", waves.Count);

        HandleWaves();

        UpdateShader();
    }


    /// <summary>
    /// Create and emit a wave which is updates over time.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="radius"></param>
    /// <param name="speed"></param>
    /// <param name="color"></param>
    public void EmitWave(Vector3 worldPos, float radius, float speed, Color color)
    {
        if (waves.Count >= maximumWaves) return;

        var wave = new Wave(worldPos, radius, speed, color);

        waves.Insert(0, wave);
    }

    /// <summary>
    /// Update waves progress and remove olders ones.
    /// </summary>
    private void HandleWaves()
    {
        // Calculate speeds progression
        for (int i = 0; i < waves.Count; i++)
        {
            var wave = waves[i];
            wave.UpdateDistance(Time.deltaTime);
        }

        waves = waves.Where(wave => wave.Distance < Wave.MAXIMUM_DISTANCE).ToList();
    }


    /// <summary>
    /// Update shader with array of values extracted from the wave list.
    /// </summary>
    private void UpdateShader()
    {
        if (waves.Count > 0)
        {
            Vector4[] shaderOrigins = GetShaderOrigins(waves);
            float[] radius = waves.Select(wave => wave.Radius).ToArray();
            Vector4[] colors = waves.Select(wave => (Vector4) wave.Color).ToArray();

            material.SetVectorArray("_Origins", shaderOrigins);
            material.SetFloatArray("_Radius", radius);
            material.SetVectorArray("_Colors", colors);
        }
        else
        {
            material.SetVectorArray("_Origins", EMPTY_VECTOR_ARRAY);
            material.SetFloatArray("_Radius", EMPTY_FLOAT_ARRAY);
            material.SetVectorArray("_Colors", EMPTY_VECTOR_ARRAY);
        }
    }

    /// <summary>
    /// Extract Vector3[] of origins and transform into Vector4[] with wave distance for w coordinate.
    /// </summary>
    /// <param name="waves"></param>
    /// <returns></returns>
    private Vector4[] GetShaderOrigins(List<Wave> waves)
    {
        var shaderOrigins = new Vector4[waves.Count];

        Vector3[] origins = waves.Select(wave => wave.Origin).ToArray();
        float[] distances = waves.Select(wave => wave.Distance).ToArray();

        for (int i = 0; i < origins.Count(); i++)
        {
            var origin = origins[i];
            var distance = distances[i];

            shaderOrigins[i] = new Vector4(origin.x, origin.y, origin.z, distance);
        }

        return shaderOrigins.ToArray();
    }
}