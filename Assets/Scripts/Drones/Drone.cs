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
    [SerializeField] private Transform sentryPointsParent;
    [SerializeField] private float sentrySecondsPerPoint = 5f;
    [SerializeField] private float sentryRotationSpeed = 200f;
    private int sentryPointsIndex;
    private float lastPointChangeTime;
    private Transform targetPoint;

    [Header("Patroller params")]
    [SerializeField] private Transform patrollingPointsParent;
    [SerializeField] private float patrollingSpeed = 5f;

    private void Awake()
    {
        lastPointChangeTime = Time.time;
        sentryPointsIndex = -1;
    }

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
        // no need to rotate on self if there's only 1 position to guard!
        if (this.sentryPointsParent.childCount > 1)
        {
            // change position we're looking at every X seconds, if we have more than 1 child to look at
            if (sentryPointsIndex < 0 || Time.time - lastPointChangeTime >= sentrySecondsPerPoint)
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
                this.targetPoint = sentryPointsParent.GetChild(sentryPointsIndex);

                this.lastPointChangeTime = Time.time;
            }

            // if we have a target point, rotate towards it
            if (this.targetPoint)
            {
                this.body.transform.rotation = Quaternion.RotateTowards(this.body.transform.rotation, Quaternion.LookRotation(targetPoint.position - transform.position), sentryRotationSpeed * Time.deltaTime);
            }
        }
    }

    private void DoPatrollerBehaviour()
    {

    }

    public void KillThePlayer()
    {
        // TODO: kill the player
        Debug.Log("Kill the player!");
    }
}
