using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DroneEngine : MonoBehaviour, IEngine
{
    [SerializeField] private float maxPower = 4f;
    public void InitEngine()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateEngine(Rigidbody rb, DronePlayer input)
    {
        Vector3 upVector = transform.up;
        upVector.x = 0;
        upVector.z = 0;
        float diff = 1 - upVector.magnitude;
        float finalDiff = diff * Physics.gravity.magnitude;

        Vector3 engineForce = Vector3.zero;
        engineForce = transform.up * ((rb.mass * Physics.gravity.magnitude + finalDiff) + (input.Throttle * maxPower)) / 4;
        rb.AddForce(engineForce, ForceMode.Force);
    }
}
