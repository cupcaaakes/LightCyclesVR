using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    public OVRInput.Controller LeftController = OVRInput.Controller.LTouch;
    public OVRInput.Controller RightController = OVRInput.Controller.RTouch;

    [SerializeField] private Transform handlebarCenter; // The center point of the handlebar
    public float SteeringAngle { get; private set; }
    public float TiltAngle { get; private set; }

    private void Update()
    {
        // Get controller positions in local space
        Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(LeftController);
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(RightController);

        // Calculate the horizontal steering angle
        Vector3 handlebarDirection = rightControllerPosition - leftControllerPosition;
        SteeringAngle = Vector3.SignedAngle(Vector3.right, handlebarDirection, Vector3.forward);

        // Calculate the tilt angle based on vertical difference
        float verticalDifference = rightControllerPosition.y - leftControllerPosition.y;
        TiltAngle = Mathf.Clamp(verticalDifference * 45f, -45f, 45f);

        // Apply the tilt angle only on the local X-axis
        if (handlebarCenter != null)
        {
            Vector3 currentRotation = handlebarCenter.localEulerAngles; // Get the current local rotation
            currentRotation.x = TiltAngle; // Update only the X-axis for tilt
            handlebarCenter.localEulerAngles = currentRotation; // Apply the new rotation
        }
    }
}
