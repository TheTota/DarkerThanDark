using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveController waveController;
    [SerializeField] private float delayBetweenFootsteps = .7f;
    [SerializeField] private float offsetBetweenSteps = .3f;

    private Vector3 lastPos;
    private float lastStepTime;
    private bool steppingRight = true;

    private void Start()
    {
        lastPos = this.transform.position;
        lastStepTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Player "emits" a wave
        if (Input.GetMouseButtonDown(0))
        {
            SpawnWaveOnPlayerPos();
        }
    }

    private void FixedUpdate()
    {
        // Trigger footsteps if moving and delay between footsteps is ok
        if (Vector3.SqrMagnitude(this.transform.position - lastPos) > .001f && Time.time - lastStepTime >= delayBetweenFootsteps)
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
        waveController.CreateWave(this.transform.position); // TODO: add radius, speed, color params
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
            waveController.CreateWave(hit.point); // TODO: add radius, speed, color params
            // TODO: play footstep sound here
        }
    }

    /// <summary>
    /// (UNUSED) Spawns a wave starting on the aimed position (aim with crosshair).
    /// </summary>
    private void SpawnWaveOnCrosshairPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info))
        {
            var worldPos = info.point;

            waveController.CreateWave(worldPos);
        }
    }
}