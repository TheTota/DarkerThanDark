using System;
using UnityEngine;

[Serializable]
public class Wave
{
    [SerializeField] private Vector3 origin;

    [SerializeField] private float radius;
    [SerializeField] private float speed;
    [SerializeField] private float distance;

    [SerializeField] private Color color;

    public Vector3 Origin { get => origin; }
    public float Radius { get => radius; }
    public float Distance { get => distance; }
    public Color Color { get => color; }

    public const float MAXIMUM_DISTANCE = 1;

    public Wave(Vector3 origin, float radius) : this(origin, radius, 1, Color.white)
    {
        this.origin = origin;
        this.radius = radius;
    }

    public Wave(Vector3 origin, float radius, float speed, Color color)
    {
        this.origin = origin;
        this.radius = radius;
        this.speed = speed;
        this.color = color;

        distance = 0;
    }

    public void UpdateDistance(float timeSpeedFactor)
    {
        float updatedDistance = distance + speed * timeSpeedFactor / radius;

        distance = Mathf.Min(updatedDistance, MAXIMUM_DISTANCE);
    }
}
