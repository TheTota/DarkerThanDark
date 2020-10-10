using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronevisionDetection : MonoBehaviour
{
    [SerializeField] private Drone drone;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        this.playerTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector3.SqrMagnitude(this.transform.position - this.playerTransform.position);
        if (distanceFromPlayer < 50f)
        {
            Debug.DrawLine(this.transform.position, this.playerTransform.position, Color.green);
        }
        else
        {
            Debug.DrawLine(this.transform.position, this.playerTransform.position, Color.red);
        }
    }
}
