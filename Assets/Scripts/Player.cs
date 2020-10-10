using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private WaveController waveController;

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

    private void Start()
    {
        lastPos = this.transform.position;
        lastStepTime = Time.time;
        lastShoutTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Player "emits" a wave
        if (Input.GetMouseButtonDown(0) && Time.time - lastShoutTime >= delayBetweenShouts)
        {
            SpawnWaveOnPlayerPos();
            lastShoutTime = Time.time;
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
        waveController.EmitWave(this.transform.position, 35, 4, Color.yellow);
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
            waveController.EmitWave(hit.point, 15, 4, Color.white); 
            // TODO: play footstep sound here
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// (UNUSED) Spawns a wave starting on the aimed position (aim with crosshair).
    /// </summary>
    private void SpawnWaveOnCrosshairPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info))
        {
            var worldPos = info.point;

            waveController.EmitWave(worldPos, 15, 6, Color.white);
        }
    }
#endif
}