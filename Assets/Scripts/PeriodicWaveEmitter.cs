using FMODUnity;
using System;
using UnityEngine;

public class PeriodicWaveEmitter : MonoBehaviour
{
    private WaveController waveController;

    [Header("Waves")]
    [SerializeField] private float secondsBetweenWaves = 2f;

    [SerializeField] private float wavesRadius = 10f;
    [SerializeField] private float wavesSpeed = 4f;
    [SerializeField] private Color wavesColor = Color.white;

    [Header("Directional")]
    [SerializeField] private Vector3 direction = Vector3.zero;
    [SerializeField] private float angle = 0f;

    private float lastWaveTime;
    private bool alert;

    [Header("FMOD")]
    [FMODUnity.EventRef]
    public string waveSound;


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
            // directional wave
            var directionalWave = new Wave(transform.position, wavesRadius, wavesSpeed, wavesColor);
            Action<Wave> EmitDirectionalWave = GetEmitWaveBehavior(true);
            EmitDirectionalWave(directionalWave);

            // proximity wave 
            //var proxWave = new Wave(transform.position, wavesRadius / 2f, wavesSpeed, wavesColor);
            //Action<Wave> EmitProxWave = GetEmitWaveBehavior(false);
            //EmitProxWave(proxWave);

            lastWaveTime = Time.time;

            RuntimeManager.PlayOneShotAttached(waveSound, gameObject);
        }
    }

    public void SetValues(float secondsBetweenWaves, float wavesRadius, float wavesSpeed, Color wavesColor)
    {
        this.secondsBetweenWaves = secondsBetweenWaves;
        this.wavesRadius = wavesRadius;
        this.wavesSpeed = wavesSpeed;
        this.wavesColor = wavesColor;
    }

    public void SetDirectionalValues(Vector3 direction, float angle)
    {
        this.direction = direction;
        this.angle = angle;
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

    public void SetWavesColor(Color c)
    {
        this.wavesColor = c;
    }

    public float GetDirectionalAngle()
    {
        return this.angle;
    }

    private Action<Wave> GetEmitWaveBehavior(bool directional)
    {
        if (directional)
        {
            return wave => waveController.EmitDirectionalWave(wave, direction, angle);
        }
        else
        {
            return wave => waveController.EmitWave(wave);
        }

    }

    public void SetAlert(bool state)
    {
        if (state && !alert)
        {
            alert = true;
            this.SetValues(secondsBetweenWaves / 3, wavesRadius, wavesSpeed * 1.5f, Color.red);
            this.lastWaveTime = this.secondsBetweenWaves;
        }
        else if (!state && alert)
        {
            alert = false;
            this.SetValues(secondsBetweenWaves * 3, wavesRadius, wavesSpeed / 1.5f, Color.yellow);
        }
    }
}