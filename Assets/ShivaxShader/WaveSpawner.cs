using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveController waveController;

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info))
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPos = info.point;

                waveController.CreateWave(worldPos);
            }
        }
    }
}