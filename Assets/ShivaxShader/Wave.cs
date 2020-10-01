using System;
using UnityEngine;

[Serializable]
public class Wave
{
    [SerializeField] private Vector3 origin;
    [SerializeField] private float radius;
    [SerializeField] private float distance;

    public Vector3 Origin { get => origin; }
    public float Distance { get => distance; }

    public const float MAXIMUM_DISTANCE = 1;

    public Wave(Vector3 origin, float radius)
    {
        this.origin = origin;
        this.radius = radius;
        distance = 0;
    }

    public void UpdateDistance(float speed)
    {
        float updatedDistance = distance + speed * Time.deltaTime / radius;

        distance = Mathf.Min(updatedDistance, MAXIMUM_DISTANCE);
    }
}
