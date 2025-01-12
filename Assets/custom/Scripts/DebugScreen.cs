using UnityEngine;
using TMPro;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] private SteeringWheel steeringWheel; // Reference to the SteeringWheel class
    [SerializeField] private TMP_Text displayText; // Assign in the Inspector
    [SerializeField] private NodeManager nodeManager;

    private void Update()
    {
        if (steeringWheel == null || displayText == null)
        {
            displayText.text = "SteeringWheel or displayText not assigned!";
            return;
        }

        // Display controller positions and angles
        Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(steeringWheel.LeftController);
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(steeringWheel.RightController);

        displayText.text =/*
            $"Left Controller Position: {FormatVector3(leftControllerPosition)}\n" +
            $"Right Controller Position: {FormatVector3(rightControllerPosition)}\n" +
            $"Steering Angle: {steeringWheel.SteeringAngle:F2}°\n" +
            $"Tilt Angle: {steeringWheel.TiltAngle:F2}°";
    */
            $"Nodes spawned: {nodeManager.NodeCounter}\n" +
            $"DragNode position: {nodeManager.DragNode.transform.position}";
    }

    private string FormatVector3(Vector3 vector)
    {
        return string.Format("({0:F2}, {1:F2}, {2:F2})", vector.x, vector.y, vector.z);
    }
}
