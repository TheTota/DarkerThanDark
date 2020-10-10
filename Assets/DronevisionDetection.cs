using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronevisionDetection : MonoBehaviour
{
    [SerializeField] private Drone drone;
    [Header("Vision")]
    [SerializeField] private float visionRange = 50f;
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

        // Check if player in line of sight and at the right distance
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, visionRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawLine(this.transform.position, hit.point, Color.green);
                drone.KillThePlayer();
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
