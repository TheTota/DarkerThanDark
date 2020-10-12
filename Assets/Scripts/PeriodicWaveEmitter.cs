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
            waveController.EmitWave(this.transform.position, wavesRadius, wavesSpeed, wavesColor);
            lastWaveTime = Time.time;
        }
    }

}
