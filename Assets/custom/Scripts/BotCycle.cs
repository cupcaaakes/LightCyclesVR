using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotCycle : MonoBehaviour
{
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

            //transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0, 100, 0), Time.deltaTime);

            // Rotate the motorcycle based on the steering angle
            
            float steeringAngle = Random.Range(-45f, 45f);
            Quaternion targetRotation = (steeringAngle < deadZone && steeringAngle > -deadZone) ? Quaternion.identity : Quaternion.Euler(0, (steeringAngle * turnSpeed), 0);
            DeadZoneActive = (steeringAngle < deadZone && steeringAngle > -deadZone) ? true : false;

            // Apply the yaw rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * targetRotation, Time.deltaTime);
            
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Wall wallComponent = other.gameObject.GetComponent<Wall>();
            if (wallComponent == null || ((other.gameObject.GetComponent<Wall>().colorBlue == false && other.gameObject.GetComponent<Wall>().isActive == true) || other.gameObject.GetComponent<Wall>().colorBlue == true))
            {
                Debug.Log($"Collided with {other.gameObject.name}");
                forwardSpeed = 0;
                dead = true;
            }
        }
    }
}
