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

    private void Start()
    {
        waves = new List<Wave>();

        // Set the maximum size of the arrays used by the shader 
        material.SetVectorArray("_Origins", new Vector4[shaderArraySizeLimit]);
        material.SetFloatArray("_Radius", new float[shaderArraySizeLimit]);
        material.SetVectorArray("_Colors", new Vector4[shaderArraySizeLimit]);
    }

    private void Update()
    {
        material.SetInt("_WavesCount", waves.Count);

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
    }


    /// <summary>
    /// Update shader with array of values extracted from the wave list.
    /// </summary>
    private void UpdateShader()
    {
        if (waves.Count > 0)
        {
            Vector4[] shaderOrigins = GetShaderOrigins(waves);
            float[] radius = ExtractArray(waves, wave => wave.Radius);
            Vector4[] colors = ExtractArray(waves, wave => (Vector4)wave.Color);

            material.SetVectorArray("_Origins", shaderOrigins);
            material.SetFloatArray("_Radius", radius);
            material.SetVectorArray("_Colors", colors);
        }
    }

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

        Vector3[] origins = ExtractArray(waves, wave => wave.Origin);
        float[] distances = ExtractArray(waves, wave => wave.Distance);

        for (int i = 0; i < origins.Count(); i++)
        {
            var origin = origins[i];
            var distance = distances[i];

            shaderOrigins[i] = new Vector4(origin.x, origin.y, origin.z, distance);
        }

        return shaderOrigins.ToArray();
    }

    /// <summary>
    /// Generic method which extracts an array of values by specifying a selector which is called on each elements of the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="waves"></param>
    /// <param name="selector">Evaluated on each item of the waves list</param>
    /// <returns></returns>
    private static T[] ExtractArray<T>(List<Wave> waves, Func<Wave, T> selector)
    {
        var values = new List<T>();

        foreach (var wave in waves)
        {
            var extractedValue = selector(wave);

            values.Add(extractedValue);
        }

        return values.ToArray();
    }

}