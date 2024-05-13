using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(DronePlayer))]
public class DroneController : DroneRigidBody
{
    [SerializeField] private float minMaxPitch = 30f;
    [SerializeField] private float minMaxRoll = 30f;
    [SerializeField] private float yawPower = 4f;

    private DronePlayer droneInput;
    private List<IEngine> engine = new List<IEngine>();

    private float yaw;
    private float finalPitch;
    private float finalRoll;
    private float finalYaw;
    private void Awake()
    {
        droneInput = GetComponent<DronePlayer>();
        engine = GetComponentsInChildren<IEngine>().ToList<IEngine>();
    }
    protected override void HandlePhysics()
    {
        HandleEngines();
        HandleControls();
    }

    protected virtual void HandleEngines()
    {
        //rigidBody.AddForce(Vector3.up * (rigidBody.mass * Physics.gravity.magnitude));
        foreach (IEngine engine in engine)
        {
            engine.UpdateEngine(rigidBody,droneInput);
        }
    }

    protected virtual void HandleControls()
    {
        float pitch = (droneInput.PitchAndRoll.y * minMaxPitch);
        float roll = -droneInput.PitchAndRoll.x * minMaxRoll;
        yaw += droneInput.Yaw * yawPower;

        finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * 2f);
        finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * 2f);
        finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * 2f);

        Quaternion rot = Quaternion.Euler(finalPitch, finalYaw, finalRoll);
        rigidBody.MoveRotation(rot);
    }
}
