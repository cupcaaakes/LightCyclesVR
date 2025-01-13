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

    private void Awake()
    {
        DeadZoneActive = true;
    }

    private void Update()
    {
        // Move the motorcycle forward at a constant speed
        transform.Translate(Vector3.right * forwardSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0, 100, 0), Time.deltaTime);

        // Rotate the motorcycle based on the steering angle
        if (steeringWheel != null)
        {
            float steeringAngle = steeringWheel.SteeringAngle;
            Quaternion targetRotation = (steeringAngle < deadZone && steeringAngle > -deadZone)? Quaternion.identity : Quaternion.Euler(0, -(steeringAngle * turnSpeed), 0);
            DeadZoneActive = (steeringAngle < deadZone && steeringAngle > -deadZone) ? true : false;

            // Apply the yaw rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * targetRotation, Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the "Collidable" tag
        if (collision.gameObject.CompareTag("Collidable"))
        {
            // Stop the motorcycle
            forwardSpeed = 0;
            Debug.Log("Collision with a Collidable object occurred.");
        }
    }
}
