using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDetectionProximity : MonoBehaviour
{
    [SerializeField] private Drone drone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            drone.KillThePlayer();
        }
    }
}
