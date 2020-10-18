using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicWaveEmitter : MonoBehaviour
{
    private WaveController waveController;

    [Header("Waves")]
    [SerializeField] private float secondsBetweenWaves = 2f;
    [SerializeField] private float wavesRadius = 10f;
    [SerializeField] private float wavesSpeed = 4f;
    [SerializeField] private Color wavesColor = Color.white;
    private float lastWaveTime;


    private void Start()
    {
        waveController = WaveController.Instance;
        lastWaveTime = Time.time;
    }

    private void Update()
    {
        // Emit waves
        if (Time.time - lastWaveTime >= secondsBetweenWaves)
        {
            Wave w = new Wave(this.transform.position, wavesRadius, wavesSpeed, wavesColor);
            waveController.EmitWave(w);
            lastWaveTime = Time.time;
        }
    }
    public void SetValues(float secondsBetweenWaves, float wavesRadius, float wavesSpeed, Color wavesColor)
    {
        this.secondsBetweenWaves = secondsBetweenWaves;
        this.wavesRadius = wavesRadius;
        this.wavesSpeed = wavesSpeed;
        this.wavesColor = wavesColor;
    }

    public float GetSecondsBetweenWaves()
    {
        return this.secondsBetweenWaves;
    }

    public float GetWavesRadius()
    {
        return this.wavesRadius;
    }

    public float GetWavesSpeed()
    {
        return this.wavesSpeed;
    }

    public Color GetWavesColor()
    {
        return this.wavesColor;
    }
}
