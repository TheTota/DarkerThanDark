using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class Goal : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2f;

    [Header("FMOD")]
    [FMODUnity.EventRef]
    public string exitAmbianceEvent;
    [FMODUnity.EventRef]
    public string exitAmbianceEndEvent;

    FMOD.Studio.EventInstance exitAmbiance;
    
    private void Start()
    {
        // FMOD Exit sound ambiance instantiation
        exitAmbiance = RuntimeManager.CreateInstance(exitAmbianceEvent);
        RuntimeManager.AttachInstanceToGameObject(exitAmbiance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        exitAmbiance.start();
    }

    private void Update()
    {
        // Rotate on self
        this.transform.Rotate(rotationSpeed * new Vector3(.2f, .2f, .2f) * Time.deltaTime);
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

    /// <summary>
    /// Load next level if possible. 
    /// If we've completed the last level, load the GG scene.
    /// </summary>
    private static void LoadNextLevel()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentBuildIndex + 1 <= SceneManager.sceneCountInBuildSettings)
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
