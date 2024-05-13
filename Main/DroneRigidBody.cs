using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneRigidBody : MonoBehaviour
{
    [SerializeField] private float droneWeight = 1f;

    public Rigidbody rigidBody;

    protected float startDrag;
    protected float startAngularDrag;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.mass = droneWeight;
            startDrag = rigidBody.drag;
            startAngularDrag = rigidBody.angularDrag;
        }
    }

    private void FixedUpdate()
    {
        if (!rigidBody)
        {
            return;
        }
        HandlePhysics();
    }

    protected virtual void HandlePhysics() { }
}
