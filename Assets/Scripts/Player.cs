using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private WaveController waveController;
    private FirstPersonAIO fpsController;
    private Camera mainCam;

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
    
    // Game Over
    [Header("Game Over")]
    [SerializeField] private float cameraRotationSpeed = 100f;
    public bool IsGameOver { get; set; }
    private Transform killerDrone;
    
    private void Awake()
    {
        this.fpsController = GetComponent<FirstPersonAIO>();
        this.mainCam = Camera.main;
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
        else if (IsGameOver)
        {
            this.mainCam.transform.rotation = Quaternion.RotateTowards(this.mainCam.transform.rotation, Quaternion.LookRotation(this.killerDrone.position - transform.position), this.cameraRotationSpeed * Time.deltaTime);
        }

        // Escape to return to leave the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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
        // TODO: play "scream" sound here (if that's what we wanna do)
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
            // TODO: play footstep sound here
        }
    }
    
    /// <summary>
    /// Turns player towards drone that spotted him, then restarts the level after a delay.
    /// </summary>
    /// <param name="killerDrone"></param>
    public void GameOver(Transform killerDrone)
    {
        IsGameOver = true;

        // block cam and look at drone who killed player
        this.fpsController.enableCameraMovement = false;
        this.fpsController.playerCanMove = false;
        this.killerDrone = killerDrone;

        // UI display "GameOver"
        GameObject.FindGameObjectWithTag("InGameUI").GetComponent<Animator>().SetBool("GameOver", true);
        // Restart level after delay
        StartCoroutine(RestartLevelAfterDelay(5f));
    }

    private IEnumerator RestartLevelAfterDelay(float s)
    {
        yield return new WaitForSeconds(s);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}