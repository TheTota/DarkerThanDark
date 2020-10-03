using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private WaveController waveController;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Waves")]
    [SerializeField] private float delayBetweenWaves = 2f;
    private float lastWaveTime;

    private void Start()
    {
        lastWaveTime = Time.time;
    }

    private void Update()
    {
        // Rotate on self
        this.transform.Rotate(rotationSpeed * new Vector3(.2f, .2f, .2f) * Time.deltaTime);

        // Emit waves
        if (Time.time - lastWaveTime >= delayBetweenWaves)
        {
            waveController.EmitWave(this.transform.position, 10, 4, Color.green);
            lastWaveTime = Time.time;
        }
    }

    /// <summary>
    /// When player enters trigger, end the level.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level complete");
            // TODO: Load next level or something
        }
    }
}
