using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDetectionProximity : MonoBehaviour
{
    [SerializeField] private Drone drone;
    [SerializeField] private float detectionTime = 1f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player p = other.transform.GetComponent<Player>();
            p.IncreaseAwarenessValue(Time.deltaTime / detectionTime, drone.body.transform, drone);
        }
    }
}
