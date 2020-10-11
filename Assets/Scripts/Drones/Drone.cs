using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroneType
{
    Sentry,
    Patroller
}

public class Drone : MonoBehaviour
{
    public Transform body;
    [SerializeField] private DroneType type;

    [Header("Sentry params")]
    [SerializeField] private float sentrySecondsPerPoint = 5f;
    [SerializeField] private Transform sentryPointsParent;
    private int sentryPointsIndex;
    private float lastPointChangeTime;

    // Update is called once per frame
    void Update()
    {
        if (type == DroneType.Sentry)
        {
            DoSentryBehaviour();
        }
        else if (type == DroneType.Patroller)
        {
            DoPatrollerBehaviour();
        }
    }

    private void DoSentryBehaviour()
    {
        // change position we're looking at every X seconds
        if (Time.time - lastPointChangeTime >= sentrySecondsPerPoint)
        {
            // set sentry point index
            if (sentryPointsIndex + 1 < sentryPointsParent.childCount)
            {
                sentryPointsIndex++;
            }
            else
            {
                sentryPointsIndex = 0;
            }

            // look at new position
            this.body.transform.LookAt(sentryPointsParent.GetChild(sentryPointsIndex));

            this.lastPointChangeTime = Time.time;
        }
    }

    private void DoPatrollerBehaviour()
    {
        // TODO: patroller drone behaviour
    }

    public void KillThePlayer()
    {
        // TODO: kill the player
        Debug.Log("Kill the player!");
    }
}
