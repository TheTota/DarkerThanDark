using UnityEngine;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private WaveController waveController;
    private FirstPersonAIO fpsController;
    private Camera mainCam;
    private LevelUI inGameUIGameObject;

    // Shout
    [Header("Shout")]
    [SerializeField] private float delayBetweenShouts = 1f;
    private float lastShoutTime;

    // Footsteps variables
    [Header("Footsteps")]
    [SerializeField] private float delayBetweenFootsteps = .7f;
    [SerializeField] private float offsetBetweenSteps = .3f;
    private const float FOOTSTEP_MIN_MOVEMENT_MAGNITUDE = .0001f;
    private Vector3 lastPos;
    private float lastStepTime;
    private bool steppingRight = true;

    [Header("Awareness")]
    [SerializeField] private float awarenessFullToEmptySeconds = 4f;
    private Transform killerDroneTransform;
    private Drone killerDroneScript;
    private float awarenessValue = 0f; // value between 0 and 1, 1 being detection
    private bool isAwarenessTriggered = false;

    // Game Over
    [Header("Game Over")]
    [SerializeField] private float cameraRotationSpeed = 100f;
    public bool IsGameOver { get; set; }

    private void Awake()
    {
        this.fpsController = GetComponent<FirstPersonAIO>();
        this.mainCam = Camera.main;
        this.inGameUIGameObject = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<LevelUI>();
    }


    private void Start()
    {
        waveController = WaveController.Instance;

        lastPos = this.transform.position;
        lastStepTime = Time.time;
        lastShoutTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Player "emits" a wave
        if (!IsGameOver && Input.GetMouseButtonDown(0) && Time.time - lastShoutTime >= delayBetweenShouts)
        {
            SpawnWaveOnPlayerPos();
            lastShoutTime = Time.time;
        }
        else if (IsGameOver) // Smoothly rotate cam towards killer drone
        {
            this.mainCam.transform.rotation = Quaternion.RotateTowards(this.mainCam.transform.rotation, Quaternion.LookRotation(this.killerDroneTransform.position - transform.position), this.cameraRotationSpeed * Time.deltaTime);
        }

        // Awareness system
        HandleAwareness();

        // Escape to return to leave the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Handles awareness rendering, lowering when no contact with drones and triggering game over.
    /// Called every frame in he update function.
    /// </summary>
    private void HandleAwareness()
    {
        // if awareness reaches 1, trigger game over
        if (awarenessValue == 1f && !IsGameOver)
        {
            GameOver();
        }
        else if (awarenessValue > 0f) 
        {
            // if we're not interacting with any drone, chill the waves and lower the awareness
            if (!isAwarenessTriggered)
            {
                killerDroneScript.PeriodicWavesEmitter.SetWavesColor(Color.yellow);
                awarenessValue = Mathf.Clamp01(awarenessValue - (Time.deltaTime / awarenessFullToEmptySeconds));
            }
            isAwarenessTriggered = false;

            // render awareness on UI
            inGameUIGameObject.RenderAwareness(awarenessValue);
        }
    }

    /// <summary>
    /// Increase the awareness with a given value and infos about the drone causing it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="droneTranform"></param>
    /// <param name="droneScript"></param>
    public void IncreaseAwarenessValue(float value, Transform droneTranform, Drone droneScript)
    {
        isAwarenessTriggered = true;

        killerDroneTransform = droneTranform;
        killerDroneScript = droneScript;
        killerDroneScript.PeriodicWavesEmitter.SetWavesColor(Color.red);

        this.awarenessValue = Mathf.Clamp01(this.awarenessValue + value);
    }

    private void FixedUpdate()
    {
        // Trigger footsteps if moving and delay between footsteps is ok
        if (Vector3.SqrMagnitude(this.transform.position - lastPos) > FOOTSTEP_MIN_MOVEMENT_MAGNITUDE && Time.time - lastStepTime >= delayBetweenFootsteps)
        {
            SpawnFootstepsWaves();

            // Record value for footsteps timing
            lastStepTime = Time.time;
            steppingRight = !steppingRight;
        }
        lastPos = this.transform.position;
    }

    /// <summary>
    /// Spawns a wave starting from the player's position.
    /// </summary>
    private void SpawnWaveOnPlayerPos()
    {
        waveController.EmitWave(new Wave(this.transform.position, 35, 4, Color.white));
        // play "scream" sound 
        RuntimeManager.PlayOneShot("event:/Scream", transform.position);


    }

    /// <summary>
    /// Spawns footsteps waves below the player.
    /// </summary>
    private void SpawnFootstepsWaves()
    {
        // Prepare footstep raycast
        Vector3 down = this.transform.TransformDirection(Vector3.down);
        float stepMultiplicator = this.steppingRight ? offsetBetweenSteps : -offsetBetweenSteps;
        Vector3 footstepOffset = (this.transform.right * stepMultiplicator);
        Vector3 startingPos = transform.position + footstepOffset;

        // Throw raycast and create wave on hit point
        RaycastHit hit;
        if (Physics.Raycast(startingPos, down, out hit, 3))
        {
            waveController.EmitWave(new Wave(hit.point, 5, 4, Color.white));
            // play footstep sound 
           RuntimeManager.PlayOneShot("event:/Steps", transform.position);


        }
    }

    /// <summary>
    /// Turns player towards drone that spotted him, then restarts the level after a delay.
    /// </summary>
    /// <param name="killerDrone"></param>
    public void GameOver()
    {
        IsGameOver = true;

        // block cam and look at drone who killed player
        this.fpsController.enableCameraMovement = false;
        this.fpsController.playerCanMove = false;
        this.killerDroneScript.EnterGameOverState(this);

        // UI display "GameOver"
        inGameUIGameObject.gameObject.GetComponent<Animator>().SetBool("GameOver", true);
        // Restart level after delay
        StartCoroutine(RestartLevelAfterDelay(5f));
    }

    /// <summary>
    /// Restart level after "s" seconds.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private IEnumerator RestartLevelAfterDelay(float s)
    {
        yield return new WaitForSeconds(s);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}