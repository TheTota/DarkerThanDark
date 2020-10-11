using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private Material material;


    [Header("Waves Settings")]

    [Range(1, 10)]
    [SerializeField] private int waveColorIntensity = 1;

    [Tooltip("This value represents the inner radius width")]
    [Min(0.01f)]
    [SerializeField] private float waveRadiusWidth = 0.01f;

    [Tooltip("This value represents the distance between the circle and its origin in percente")]
    [Range(0f, 1f)]
    [SerializeField] private float waveTrailWidth = 0f;

    [Tooltip("Maximum number of waves which the controller can handle")]
    [Range(1, shaderArraySizeLimit)]
    [SerializeField] private int maximumWaves = 1;
    

    [Header("Waves Debug")]

    [SerializeField] private List<Wave> waves;

    // Value fixed and synchronized with the maximum size of an array in the shader
    private const int shaderArraySizeLimit = 100;

    private static Vector4[] EMPTY_VECTOR4_ARRAY = new Vector4[shaderArraySizeLimit];
    private static float[] EMPTY_FLOAT_ARRAY = new float[shaderArraySizeLimit];

    private void Start()
    {
        waves = new List<Wave>();

        // Set the maximum size of the arrays used by the shader 
        material.SetVectorArray("_Origins", EMPTY_VECTOR4_ARRAY);
        material.SetFloatArray("_Radius", EMPTY_FLOAT_ARRAY);
        material.SetVectorArray("_Colors", EMPTY_VECTOR4_ARRAY);
    }

    private void Update()
    {
        material.SetInt("_WavesCount", waves.Count);
        material.SetFloat("_WaveColorIntensity", waveColorIntensity);
        material.SetFloat("_WaveRadiusWidth", waveRadiusWidth);
        material.SetFloat("_WaveTrailWidth", waveTrailWidth);

        HandleWaves();

        UpdateShader();

        waves = FilterWaves();
    }


    /// <summary>
    /// Create and emit a wave which is updates over time.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="radius"></param>
    /// <param name="speed"></param>
    /// <param name="color"></param>
    [Obsolete("This method is obselete. Use EmitWave(Wave) instead")]
    public void EmitWave(Vector3 worldPos, float radius, float speed, Color color)
    {
        if (waves.Count >= maximumWaves) return;

        var wave = new Wave(worldPos, radius, speed, color);

        waves.Insert(0, wave);
    }

    /// <summary>
    /// Emit a wave which is updated over time.
    /// </summary>
    /// <param name="wave"></param>
    public void EmitWave(Wave wave)
    {
        if (waves.Count >= maximumWaves) return;

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
            material.SetVectorArray("_Origins", EMPTY_VECTOR4_ARRAY);
            material.SetFloatArray("_Radius", EMPTY_FLOAT_ARRAY);
            material.SetVectorArray("_Colors", EMPTY_VECTOR4_ARRAY);
        }
    }

    /// <summary>
    /// Filter waves by removing the older ones
    /// </summary>
    /// <returns></returns>
    private List<Wave> FilterWaves()
    {
        return waves.Where(wave => wave.Distance < Wave.MAXIMUM_DISTANCE).ToList();
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