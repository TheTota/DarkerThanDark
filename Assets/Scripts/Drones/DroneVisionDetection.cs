﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneVisionDetection : MonoBehaviour
{
    [SerializeField] private Drone drone;
    [Header("Vision")]
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private float visionAngleOnEachSide = 30f;
    [SerializeField] private float visionDetectionTime = 7f;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        this.playerTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(playerTransform);
        float distanceFromPlayer = Vector3.Distance(this.transform.position, this.playerTransform.position);

        // Check if player in line of sight and at the right distance
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, visionRange) && distanceFromPlayer <= visionRange)
        {
            float angle = Vector3.Angle(this.drone.body.transform.forward, this.transform.forward);
            if (hit.transform.CompareTag("Player") && (angle <= visionAngleOnEachSide && angle >= -visionAngleOnEachSide))
            {
                Debug.DrawLine(this.transform.position, hit.point, Color.green);

                // increased awareness : the closer the player, the higher we increase awareness
                Player p = hit.transform.GetComponent<Player>();
                p.IncreaseAwarenessValue(Time.deltaTime / (visionDetectionTime * (distanceFromPlayer / visionRange)), drone.body.transform, drone);
            }
            else
            {
                Debug.DrawLine(this.transform.position, hit.point, Color.red);
            }
        }
        else
        {
            Debug.DrawLine(this.transform.position, playerTransform.position, Color.gray);
        }
    }
}
