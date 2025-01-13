using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cycle : MonoBehaviour
{
    [SerializeField] private SteeringWheel steeringWheel; // Reference to the SteeringWheel script
    [SerializeField] private float forwardSpeed = 10f; // Constant forward speed
    [SerializeField] private float turnSpeed = 5f; // Speed at which the motorcycle turns
    [SerializeField] private float deadZone = 5f;
    public bool DeadZoneActive { get; private set; }

    public bool dead;

    private void Awake()
    {
        DeadZoneActive = true;
        dead = false;
    }

    private void Update()
    {
        if (!dead)
        {
            // Move the motorcycle forward at a constant speed
            transform.Translate(Vector3.right * forwardSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0, 100, 0), Time.deltaTime);

            // Rotate the motorcycle based on the steering angle
            if (steeringWheel != null)
            {
                float steeringAngle = steeringWheel.SteeringAngle;
                Quaternion targetRotation = (steeringAngle < deadZone && steeringAngle > -deadZone) ? Quaternion.identity : Quaternion.Euler(0, -(steeringAngle * turnSpeed), 0);
                DeadZoneActive = (steeringAngle < deadZone && steeringAngle > -deadZone) ? true : false;

                // Apply the yaw rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * targetRotation, Time.deltaTime);
            }
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Wall" && (other.gameObject.GetComponent<Wall>().colorBlue == true && other.gameObject.GetComponent<Wall>().isActive == true)) 
        {
            Debug.Log($"Collided with {other.gameObject.name}");
            forwardSpeed = 0;
            dead = true;
        }
    }
}
