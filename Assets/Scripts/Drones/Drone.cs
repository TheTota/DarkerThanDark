using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float patrollingSpeed = 1f;
    private NavMeshAgent navAgent;
    private int patrollingPointsIndex;

    private bool canMove = true;

    public PeriodicWaveEmitter PeriodicWavesEmitter { get; set; }

    private void Awake()
    {
        PeriodicWavesEmitter = GetComponent<PeriodicWaveEmitter>();

        lastPointChangeTime = Time.time;
        sentryPointsIndex = -1;
        patrollingPointsIndex = 1;

        // get the nav mesh agent for patroller drones
        if (type == DroneType.Patroller)
        {
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.speed = patrollingSpeed;
            patrollingPointsParent.parent = null; // TODO: improve/clean this
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
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

        // if we have a target point, rotate towards it
        if (this.targetPoint && (type == DroneType.Sentry || !canMove))
        {
            this.body.transform.rotation = Quaternion.RotateTowards(this.body.transform.rotation, Quaternion.LookRotation(targetPoint.position - transform.position), this.sentryRotationSpeed * Time.deltaTime);
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
        }
    }

    private void DoPatrollerBehaviour()
    {
        if (this.patrollingPointsParent.childCount >= 2)
        {
            // check if destination reached
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance && (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f))
            {
                // set patroller index for next position
                if (patrollingPointsIndex + 1 < patrollingPointsParent.childCount)
                {
                    patrollingPointsIndex++;
                }
                else
                {
                    patrollingPointsIndex = 0;
                }

            }
            navAgent.SetDestination(this.patrollingPointsParent.GetChild(this.patrollingPointsIndex).position);
        }
        else
        {
            Debug.LogError("Less than 2 patrolling points found on " + this.gameObject.name + " - use a sentry drone instead.");
        }
    }

    public void EnterGameOverState(Player p)
    {
        // Stop movement & look at player (smoothly)
        this.canMove = false;
        if (this.navAgent)
        {
            this.navAgent.isStopped = true;
        }
        this.targetPoint = p.transform;

        // Emit more waves
        PeriodicWavesEmitter.SetValues(
            PeriodicWavesEmitter.GetSecondsBetweenWaves() / 4f,
            PeriodicWavesEmitter.GetWavesRadius(),
            PeriodicWavesEmitter.GetWavesSpeed(),
            Color.red
        );
    }
}
