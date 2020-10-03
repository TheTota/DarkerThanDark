using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private WaveController waveController;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Waves")]
    [SerializeField] private float delayBetweenWaves = 2f;
    [SerializeField] private float wavesRadius = 10f;
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
            waveController.EmitWave(this.transform.position, wavesRadius, 4, Color.green);
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
            LoadNextLevel();
        }
    }

    private static void LoadNextLevel()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentBuildIndex + 1 <= SceneManager.sceneCount)
        {
            SceneManager.LoadScene(currentBuildIndex + 1);
        }
        else
        {
            // TODO: handle GG
            Debug.Log("Game completed!");
        }
    }
}
