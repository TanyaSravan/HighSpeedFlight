using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class DroneAgent : Agent
{
    private DronePlayer dronePlayer;
    private Vector3 spawnPosition = new Vector3(0f, 5f, 0f);

    private Vector2 pitchAndRoll;
    private float yaw;
    private float throttle;


    [SerializeField] private float TotalEpisodeTime = 10000;
    [SerializeField] private float currTime;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Transform target;

    [SerializeField] private GameObject floor;
    private Material envMaterial;


    //Initialisatiion
    public override void Initialize()
    {

        dronePlayer = GetComponent<DronePlayer>();
        Drone_input droneInputActions = new Drone_input();
        droneInputActions.Drone.Enable();

        droneInputActions.Drone.PitchandRoll.performed += PitchandRoll_performed;
        droneInputActions.Drone.Yaw.performed += Yaw_performed;
        droneInputActions.Drone.Throttle.performed += Throttle_performed;

        envMaterial = floor.GetComponent<Renderer>().material;

    }

    //Hueristic/Model  Input
    private void Throttle_performed(InputAction.CallbackContext obj)
    {
        throttle = obj.ReadValue<float>();
    }

    private void Yaw_performed(InputAction.CallbackContext obj)
    {
        yaw = obj.ReadValue<float>();
    }

    private void PitchandRoll_performed(InputAction.CallbackContext obj)
    {
        pitchAndRoll = obj.ReadValue<Vector2>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float pitch = actions.ContinuousActions[2];
        float roll = actions.ContinuousActions[3];
        float Yaw = actions.ContinuousActions[1];
        float Trot = actions.ContinuousActions[0];
        dronePlayer.SetInputActions(pitch, Yaw, roll, Trot);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;


        continuousActions[0] = throttle;
        continuousActions[1] = yaw;
        continuousActions[2] = pitchAndRoll.x;
        continuousActions[3] = pitchAndRoll.y;
    }

    //Model Logic
    public override void OnEpisodeBegin()
    {
        transform.localPosition = spawnPosition + new Vector3(Random.Range(-5f, 5f),0, Random.Range(-5f, 5f));

        //Target position instantiation 
        float targetRandx = Random.Range(-6f, 6f);
        float targetRandz = Random.Range(-6f, 6f);
        target.localPosition = new Vector3(targetRandx, target.localPosition.y, targetRandz);

        currTime = 0;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            AddReward(100f);
            envMaterial.color = Color.green;
            EndEpisode();
        }
        if (other.gameObject.tag == "Walls") 
        {
            AddReward(-2f);
            envMaterial.color = Color.red;
            EndEpisode();
        }
        if (other.gameObject.tag == "Obstacles")
        {
            AddReward(-2f);
            envMaterial.color = Color.red;
            EndEpisode();
        }
    }

    private void EpisodeTimeExceeded()
    {
        currTime += Time.deltaTime;
        if (currTime >= TotalEpisodeTime)
        {
            AddReward(-15f);
            envMaterial.color = Color.red;
            EndEpisode();

        }
    }
    private void Update()
    {
        EpisodeTimeExceeded();
    }





}
