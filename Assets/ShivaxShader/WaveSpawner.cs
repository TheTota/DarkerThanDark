using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveController waveController;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnWaveOnPlayerPos();
        }
    }

    /// <summary>
    /// Spawns a wave starting from the player's position.
    /// </summary>
    private void SpawnWaveOnPlayerPos()
    {
        waveController.CreateWave(this.transform.position);
    }

    /// <summary>
    /// Spawns a wave starting on the aimed position (aim with crosshair).
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

    // TODO: spawn waves for footsteps

}