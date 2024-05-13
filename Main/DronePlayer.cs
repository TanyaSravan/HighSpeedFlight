using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;


[RequireComponent(typeof(PlayerInput))]
public class DronePlayer : MonoBehaviour
{
    private PlayerInput playerInput;

    private Vector2 pitchAndRoll;
    private float yaw;
    private float throttle;

    public Vector2 PitchAndRoll { get => pitchAndRoll; }
    public float Yaw { get => yaw; }
    public float Throttle { get => throttle; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

/*        
        Drone_input droneInputActions = new Drone_input();
        droneInputActions.Drone.Enable();

        droneInputActions.Drone.PitchandRoll.performed += PitchandRoll_performed;
        droneInputActions.Drone.Yaw.performed += Yaw_performed;
        droneInputActions.Drone.Throttle.performed += Throttle_performed;
*/
        
    }

    /*
    private void Throttle_performed(InputAction.CallbackContext obj)
    {
        throttle =  obj.ReadValue<float>();
        Debug.Log("Trottle: "+ throttle);
    }

    private void Yaw_performed(InputAction.CallbackContext obj)
    {
        yaw = obj.ReadValue<float>();
        Debug.Log("Yaw: "+ yaw);
    }

    private void PitchandRoll_performed(InputAction.CallbackContext obj)
    {
        pitchAndRoll = obj.ReadValue<Vector2>();
        Debug.Log("Pitch and roll: "+ pitchAndRoll);
    }
    */

    public void SetInputActions(float pitch, float yaw, float roll, float throttle)
    {
        this.pitchAndRoll = new Vector2(pitch,roll);
        this.throttle = throttle;
        this.yaw = yaw;
    }


}
