using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // Default rotation axis is Y-axis
    public float rotationSpeed = 45f; // Default rotation speed is 45 degrees per second

    void Update()
    {
        // Rotate the object based on the specified axis and speed
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}