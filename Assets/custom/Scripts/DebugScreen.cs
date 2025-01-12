using UnityEngine;
using TMPro;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] private SteeringWheel steeringWheel; // Reference to the SteeringWheel class
    [SerializeField] private TMP_Text displayText; // Assign in the Inspector
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private WallManager wallManager;
    [SerializeField] private Cycle cycle1;

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
            $"Tilt Angle: {steeringWheel.TiltAngle:F2}°" +
            
            $"InitNodePos: {NodeManager.Instance.Nodes[0].transform.position}\n" +
            $"DragNodePos: {NodeManager.Instance.DragNode}\n" +
            
            $"WallPos: {wallManager.walls[0].transform.position}\n"+
            /*
            $"Walls: {wallManager.walls.Count}\n" +
            $"TempWall: {wallManager.tempWall}" +
            *//*
            $"NSPos: {wallManager.walls[0].NodeStart.transform.position}\n" +
            $"NEPos: {wallManager.walls[0].NodeEnd.transform.position}\n" +
        
            $"NSPos: {wallManager.walls[0].mesh.vertices[2]}\n" +
             $"{wallManager.walls[0].mesh.vertices[3]}\n" +
            $"NEPos: {wallManager.walls[0].NodeEnd.transform.position}\n" +
        */
            //$"DZone1: {cycle1.DeadZoneActive}\n" +
            //$"Nodes: {NodeManager.Instance.Nodes.Count}\n" +
            $"LastNode: {NodeManager.Instance.Nodes[^1]}\n" +
            $"Walls: {wallManager.walls.Count}\n" +
            "end debug";
    }

    private string FormatVector3(Vector3 vector)
    {
        return string.Format("({0:F2}, {1:F2}, {2:F2})", vector.x, vector.y, vector.z);
    }
}
