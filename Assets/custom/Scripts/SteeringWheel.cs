using UnityEngine;
using UnityEngine.SceneManagement;

public class SteeringWheel : MonoBehaviour
{
    public OVRInput.Controller LeftController = OVRInput.Controller.LTouch;
    public OVRInput.Controller RightController = OVRInput.Controller.RTouch;

    [SerializeField] private Transform handlebarCenter; // The center point of the handlebar
    [SerializeField] private float maxSteeringAngle = 90f; // Maximum steering angle
    [SerializeField] private float tiltSensitivity = 90f; // Sensitivity for tilt calculation
    [SerializeField] private OVRInput.Button resetButton = OVRInput.Button.One; // Button to reset

    public float SteeringAngle { get; private set; }
    public float TiltAngle { get; private set; }

    private void Update()
    {
        // Get controller positions in local space
        Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(LeftController);
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(RightController);

        // Calculate the horizontal steering angle
        Vector3 handlebarDirection = rightControllerPosition - leftControllerPosition;
        SteeringAngle = Mathf.Clamp(Vector3.SignedAngle(Vector3.right, handlebarDirection, Vector3.forward), -maxSteeringAngle, maxSteeringAngle);

        // Calculate the tilt angle based on vertical difference
        float verticalDifference = rightControllerPosition.y - leftControllerPosition.y;
        TiltAngle = Mathf.Clamp(verticalDifference * tiltSensitivity, -tiltSensitivity, tiltSensitivity);

        // Apply the tilt angle only on the local X-axis
        if (handlebarCenter != null)
        {
            Vector3 targetRotation = new Vector3(TiltAngle, 0, 0); // Desired rotation
            handlebarCenter.localRotation = Quaternion.Euler(targetRotation); // Directly apply the rotation
        }
        if (OVRInput.GetDown(resetButton))
        {
            ResetGame();
        }
    }

    private void ResetGame()
    {
        // Reload the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        Debug.Log("Game reset!");
    }
}
