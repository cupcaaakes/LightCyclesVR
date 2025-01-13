using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BotCycle : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 10f; // Movement speed
    [SerializeField] private float navMeshSampleDistance = 10f; // Distance to sample valid NavMesh positions
    [SerializeField] private float maxTurnAngle = 45f; // Maximum turning angle per second
    [SerializeField] private float deadZone = 5f; // Steering angle range considered as the dead zone
    [SerializeField] private float initialForwardDuration = 5f; // Duration of initial forward-only movement

    public bool DeadZoneActive { get; private set; } // Indicates if the bot is in the dead zone

    private NavMeshAgent navMeshAgent;
    private bool movingToDestination = false;
    private Vector3 currentDestination;
    private float forwardOnlyEndTime;
    private Vector3 lastDestination; // To track previous destination

    public bool dead;

    private void Awake()
    {
        dead = false;
        DeadZoneActive = true;

        // Attach or get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        // Disable the NavMeshAgent's movement; we'll handle movement manually
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    private void Start()
    {
        // Record when the initial forward movement ends
        forwardOnlyEndTime = Time.time + initialForwardDuration;

        // Check if the agent is on the NavMesh at startup
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            transform.position = hit.position; // Snap the agent to the NavMesh
            SetNewRandomDestination();
        }
        else
        {
            Debug.LogError("BotCycle: Agent is not on a valid NavMesh at start!");
        }
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }

        if (Time.time < forwardOnlyEndTime)
        {
            // Move only forward for the initial duration
            transform.Translate(Vector3.right * forwardSpeed * Time.deltaTime);
        }
        else
        {
            // Normal navigation logic after the initial forward movement
            NavigateTowardsDestination();
        }
    }

    private void NavigateTowardsDestination()
    {
        // Move forward at a constant speed
        transform.Translate(Vector3.right * forwardSpeed * Time.deltaTime);

        // Calculate the direction and angle to the destination
        Vector3 directionToDestination = (currentDestination - transform.position).normalized;
        float steeringAngle = Vector3.SignedAngle(transform.right, directionToDestination, Vector3.up);

        // Constrain the angle to [-180°, 180°]
        if (steeringAngle > 180f)
        {
            steeringAngle -= 360f;
        }
        else if (steeringAngle < -180f)
        {
            steeringAngle += 360f;
        }

        // Update DeadZoneActive
        DeadZoneActive = Mathf.Abs(steeringAngle) <= deadZone;

        // Ensure the bot takes the shortest turn and clamp to maxTurnAngle
        float clampedSteeringAngle = Mathf.Clamp(steeringAngle, -maxTurnAngle * Time.deltaTime, maxTurnAngle * Time.deltaTime);

        // Apply the rotation
        transform.Rotate(Vector3.up, clampedSteeringAngle);

        // Check if close to the destination and set a new one if needed
        if (Vector3.Distance(transform.position, currentDestination) <= navMeshAgent.stoppingDistance)
        {
            SetNewRandomDestination();
        }
    }

    private void SetNewRandomDestination()
    {
        Vector3 randomDestination;

        // Attempt to find a valid destination far enough from the current position
        do
        {
            randomDestination = GetRandomNavMeshPoint(transform.position, navMeshSampleDistance);
        }
        while (randomDestination != Vector3.zero && Vector3.Distance(randomDestination, lastDestination) < navMeshSampleDistance / 2f);

        if (randomDestination != Vector3.zero)
        {
            lastDestination = currentDestination; // Track the previous destination
            currentDestination = randomDestination;
            Debug.Log($"New destination set: {currentDestination}");
        }
        else
        {
            Debug.LogWarning("Failed to find a valid NavMesh point for a new destination.");
        }
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 origin, float distance)
    {
        // Generate a random point in a sphere around the origin
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        // Sample the NavMesh at the random point
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, distance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero; // Return zero if no valid NavMesh position is found
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
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
