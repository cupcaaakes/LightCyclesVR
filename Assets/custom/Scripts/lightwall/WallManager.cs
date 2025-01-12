using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    /// <summary>
    /// Model of the wall. Set in the Unity Inspector.
    /// </summary>
    public GameObject wallModel;

    /// <summary>
    /// List of all walls currently in the game.
    /// </summary>
    public List<Wall> walls = new();

    /// <summary>
    /// Reference to the temporary wall currently being stretched.
    /// </summary>
    private Wall tempWall;

    /// <summary>
    /// Node that serves as the starting point for the temporary wall.
    /// </summary>
    private Node tempNodeStart;

    /// <summary>
    /// Counter for wall IDs.
    /// </summary>
    private uint wallCount = 0;

    public static WallManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Subscribe to the WallCreated event from NodeManager
        NodeManager.Instance.WallCreated += CreateWall;
        StartTempWall();
    }

    private void Update()
    {
        if (tempWall != null && NodeManager.Instance.DragNode != null)
        {
            // Dynamically update the temporary wall
            UpdateTempWall();
        }
    }

    /// <summary>
    /// Creates a new wall between two nodes.
    /// </summary>
    /// <param name="nodeStart">The start node of the wall.</param>
    /// <param name="nodeEnd">The end node of the wall.</param>
    public void CreateWall(Node nodeStart, Node nodeEnd)
    {
        Wall newWall = InstantiateWall(nodeStart, nodeEnd);
        if (newWall != null)
        {
            walls.Add(newWall);
        }
    }

    /// <summary>
    /// Instantiates a wall object between two nodes.
    /// </summary>
    /// <param name="nodeStart">The start node of the wall.</param>
    /// <param name="nodeEnd">The end node of the wall.</param>
    /// <returns>The created Wall object.</returns>
    private Wall InstantiateWall(Node nodeStart, Node nodeEnd)
    {
        Vector3 position = (nodeStart.transform.position + nodeEnd.transform.position) / 2;
        Quaternion rotation = Quaternion.identity;

        GameObject wallObj = Instantiate(wallModel, position, rotation, transform);

        if (wallObj.TryGetComponent<Wall>(out var wall))
        {
            wall.NodeStart = nodeStart;
            wall.NodeEnd = nodeEnd;
            wall.WallId = wallCount++;
            wallObj.name = $"{wall.WallId}-Wall:{nodeStart.name}-/-{nodeEnd.name}";
            return wall;
        }
        else
        {
            Debug.LogError("Wall script not found on the wall prefab!");
            Destroy(wallObj);
            return null;
        }
    }

    /// <summary>
    /// Creates or updates a temporary wall between the last placed node and the DragNode.
    /// </summary>
    public void StartTempWall()
    {
        if (tempWall == null)
        {
            Node lastNode = NodeManager.Instance.Nodes[^1]; // Get the last placed node
            Node dragNode = NodeManager.Instance.DragNode.GetComponent<Node>();

            if (lastNode != null && dragNode != null)
            {
                tempWall = InstantiateWall(lastNode, dragNode);
                tempNodeStart = lastNode;
            }
        }
    }

    /// <summary>
    /// Updates the temporary wall to stretch between the start node and DragNode.
    /// </summary>
    private void UpdateTempWall()
    {
        Node dragNode = NodeManager.Instance.DragNode.GetComponent<Node>();

        if (tempWall != null && dragNode != null)
        {
            tempWall.NodeEnd = dragNode;

            // Update wall's position
            Vector3 midPosition = (tempNodeStart.transform.position + dragNode.transform.position) / 2;
            tempWall.transform.position = midPosition;

            // Update wall's scale
            if (tempWall.NodeStart.X != tempWall.NodeEnd.X) // Along X-axis
            {
                tempWall.transform.localScale = new Vector3(
                    Mathf.Abs(tempWall.NodeStart.X - tempWall.NodeEnd.X),
                    tempNodeStart.Height,
                    tempWall.transform.localScale.z);
            }
            else // Along Z-axis
            {
                tempWall.transform.localScale = new Vector3(
                    tempWall.transform.localScale.x,
                    tempNodeStart.Height,
                    Mathf.Abs(tempWall.NodeStart.Z - tempWall.NodeEnd.Z));
            }
        }
    }

    /// <summary>
    /// Finalizes the temporary wall, making it permanent.
    /// </summary>
    public void FinalizeTempWall()
    {
        if (tempWall != null)
        {
            walls.Add(tempWall);
            tempWall = null;
            tempNodeStart = null;
        }
    }
}
