using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// A wall in three-dimensional space. Has two nodes, a mesh and a direction.
/// </summary>
public class Wall : MonoBehaviour
{
    /// <summary>
    /// Private former initial node turned permanent node.
    /// </summary>
    [SerializeField]
    private Node _nodeFormerInit;

    /// <summary>
    /// former initial node turned permanent node.
    /// </summary>
    public Node NodeStart
    {
        get { return _nodeFormerInit; }
        set
        {
            _nodeFormerInit = value;
            value.connectedWalls.Add(this);
        }
    }

    /// <summary>
    /// Private former cursor node turned permanent node.
    /// </summary>
    [SerializeField]
    private Node _nodeFormerCursor;

    /// <summary>
    /// former cursor node turned permanent node.
    /// </summary>
    public Node NodeEnd
    {
        get { return _nodeFormerCursor; }
        set
        {
            _nodeFormerCursor = value;
            value.connectedWalls.Add(this);
        }
    }

    private Mesh mesh;

    /// <summary>
    /// The axis that the wall is going along. True = X, False = Z
    /// </summary>
    public bool goesAlongX = false;

    /// <summary>
    /// Flag for checking if the direction of the wall has already been set or not.
    /// </summary>
    public bool GoesAlongSet { get; private set; }

    /// <summary>
    /// Flag for checking if the direction of the wall has already been set or not. Returns the value of bool GoesAlongSet.
    /// </summary>
    public bool IsAlongSet => GoesAlongSet;

    public float MidX { get; private set; }
    public float MidZ { get; private set; }
    public float MidHeight { get; private set; }

    [SerializeField]
    private List<Vector3> occupiedPos;

    /// <summary>
    /// The Vector3Int positions occupied by the wall.
    /// </summary>
    public List<Vector3> OccupiedPos
    {
        get { return occupiedPos; }
        private set { occupiedPos = value; }
    }
    /*
    [SerializeField]
    private List<Vector3Int> occupiedPos;
    

    /// <summary>
    /// The Vector3Int positions occupied by the wall.
    /// </summary>
    public List<Vector3Int> OccupiedPos
    {
        get { return occupiedPos; }
        private set { occupiedPos = value; }
    }
    */

    /// <summary>
    /// Pretty self-explanatory. The ID of the wall. You knew that already, come on.
    /// </summary>
    public uint WallId { get; set; }



    /// <summary>
    /// Called immediately upon wall creation, before Start().
    /// </summary>
    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            Debug.LogError("ERROR: MeshFilter component missing!");
            return;
        }

        mesh = meshFilter.mesh;

        // Create an array to hold the local positions of the vertices 
        Vector3[] localVertices = new Vector3[4];

        // Get the transform component of the game object
        Transform objectTransform = gameObject.transform;

        // Calculate the wall's X coordinate.
        // This is the average of the X coordinates of NodeFormerInit and NodeFormerCursor.
        MidX = (NodeStart.X + NodeEnd.X) / 2;

        // Calculate the wall's Z coordinate.
        // This is the average of the Z coordinates of NodeFormerInit and NodeFormerCursor.
        MidZ = (NodeStart.Z + NodeEnd.Z) / 2;

        // Calculate the wall's height midpoint (within Y coordinate).
        // This is the average of the heights of NodeFormerInit and NodeFormerCursor, 
        // then divided by 2 to adjust for the midpoint height.
        MidHeight = ((NodeStart.Height + NodeEnd.Height) / 2) / 2;

        // Set the position of the wall's objectTransform to the calculated midpoint.
        objectTransform.position = new Vector3(MidX, MidHeight, MidZ);

        // Determine the orientation of the wall based on the Z coordinates of the nodes.
        if (NodeStart.X != NodeEnd.X)
        {
            // If MidX is not a whole number, it implies that the X coordinates of the nodes
            // are different, indicating the wall extends along the X axis.
            goesAlongX = true;

            // No additional rotation is needed, as the default orientation aligns along the X axis.
        }
        else
        {
            // If midZ is a whole number, it implies that the Z coordinates of the nodes
            // are the same, indicating the wall extends along the Z axis.
            goesAlongX = false;

            // Rotate the object by 90 degrees around the Y axis to align it along the Z axis.
            objectTransform.rotation = Quaternion.Euler(0, 90, 0);
        }

        Vector3 worldVertBotLeft, worldVertTopLeft, worldVertBotRight, worldVertTopRight;

       
        // Set the world positions of the vertices. Not sure why I need to disable a warning here since these are pretty important for self-explanatory reasons.
#pragma warning disable IDE0090
        worldVertBotLeft = new Vector3(NodeStart.X, 0, NodeStart.Z);                // Bottom-left vertex
        worldVertTopLeft = new Vector3(NodeStart.X, NodeStart.Height, NodeStart.Z); // Top-left vertex
        worldVertBotRight = new Vector3(NodeEnd.X, 0, NodeEnd.Z);                   // Bottom-right vertex
        worldVertTopRight = new Vector3(NodeEnd.X, NodeEnd.Height, NodeEnd.Z);      // Top-right vertex

        if (goesAlongX)
        {
                gameObject.transform.localScale = new Vector3(Mathf.Abs(NodeStart.X - NodeEnd.X), NodeStart.Height, gameObject.transform.localScale.z);
        }
        else
        {

                gameObject.transform.localScale = new Vector3(Mathf.Abs(NodeEnd.Z - NodeStart.Z), NodeStart.Height, gameObject.transform.localScale.z);
        }

        OccupiedPos = GetOccupiedPos(goesAlongX);

        // Wall setup complete; set the GoesAlongSet flag to true.
        GoesAlongSet = true;
    }

    /// <summary>
    /// Updates the position of the wall when it is moved.
    /// </summary>
    public void UpdatePos()
    {
        GoesAlongSet = false;

        // Create an array to hold the local positions of the vertices 
        Vector3[] localVertices = new Vector3[4];

        // Get the transform component of the game object
        Transform objectTransform = gameObject.transform;

        // Calculate the wall's X coordinate.
        // This is the average of the X coordinates of NodeFormerInit and NodeFormerCursor.
        MidX = ((float)NodeStart.X + (float)NodeEnd.X) / 2;

        // Calculate the wall's Z coordinate.
        // This is the average of the Z coordinates of NodeFormerInit and NodeFormerCursor.
        MidZ = ((float)NodeStart.Z + (float)NodeEnd.Z) / 2;

        // Calculate the wall's height midpoint (within Y coordinate).
        // This is the average of the heights of NodeFormerInit and NodeFormerCursor, 
        // then divided by 2 to adjust for the midpoint height.
        MidHeight = (((float)NodeStart.Height + (float)NodeEnd.Height) / 2) / 2;

        // Set the position of the wall's objectTransform to the calculated midpoint.
        objectTransform.position = new Vector3(MidX, MidHeight, MidZ);

        // Determine the orientation of the wall based on the Z coordinates of the nodes.
        if (NodeStart.X != NodeEnd.X)
        {
            // If MidX is not a whole number, it implies that the X coordinates of the nodes
            // are different, indicating the wall extends along the X axis.
            goesAlongX = true;

            // No additional rotation is needed, as the default orientation aligns along the X axis.
        }
        else
        {
            // If midZ is a whole number, it implies that the Z coordinates of the nodes
            // are the same, indicating the wall extends along the Z axis.
            goesAlongX = false;

            // Rotate the object by 90 degrees around the Y axis to align it along the Z axis.
            objectTransform.rotation = Quaternion.Euler(0, 90, 0);
        }

        Vector3 worldVertBotLeft, worldVertTopLeft, worldVertBotRight, worldVertTopRight;

#pragma warning disable IDE0090
        worldVertBotLeft = new Vector3(NodeStart.X, 0, NodeStart.Z);                // Bottom-left vertex
        worldVertTopLeft = new Vector3(NodeStart.X, NodeStart.Height, NodeStart.Z); // Top-left vertex
        worldVertBotRight = new Vector3(NodeEnd.X, 0, NodeEnd.Z);                   // Bottom-right vertex
        worldVertTopRight = new Vector3(NodeEnd.X, NodeEnd.Height, NodeEnd.Z);      // Top-right vertex

        if (goesAlongX)
        {
            gameObject.transform.localScale = new Vector3(Mathf.Abs(NodeStart.X - NodeEnd.X), NodeStart.Height, gameObject.transform.localScale.z);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(Mathf.Abs(NodeEnd.Z - NodeStart.Z), NodeStart.Height, gameObject.transform.localScale.z);
        }

        OccupiedPos = GetOccupiedPos(goesAlongX);

        // Wall setup complete; set the GoesAlongSet flag to true.
        GoesAlongSet = true;
        NodeStart.name = "Node" + NodeStart.transform.position;
        NodeEnd.name = "Node" + NodeEnd.transform.position;
        this.name = WallId + "-" + "Wall" + NodeStart.name + "-/-" + NodeEnd.name;
    }

    /// <summary>
    /// Calculates a list of positions occupied by a wall either along the X-axis or Z-axis.
    /// </summary>
    /// <param name="goesAlongX">Determines if the wall extends along the X-axis. If false, it extends along the Z-axis.</param>
    /// <returns>A list of Vector3Int positions occupied by the wall.</returns>
    private List<Vector3> GetOccupiedPos(bool goesAlongX)
    {
        List<Vector3> positions = new();

        // If the structure does not go along the X-axis, it extends along the Z-axis.
        if (!goesAlongX)
        {
            // Determine the start and end points along the Z-axis.
            float startZ = Mathf.Min(NodeStart.Z, NodeEnd.Z);
            float endZ = Mathf.Max(NodeStart.Z, NodeEnd.Z);

            // Loop through each point along the Z-axis and add it to the list of occupied positions.
            for (float z = startZ; z <= endZ; z++)
            {
                positions.Add(new Vector3(NodeStart.X, 0, z));
            }
        }
        else // If the structure extends along the X-axis.
        {
            // Determine the start and end points along the X-axis.
            float startX = Mathf.Min(NodeStart.X, NodeEnd.X);
            float endX = Mathf.Max(NodeStart.X, NodeEnd.X);

            // Loop through each point along the X-axis and add it to the list of occupied positions.
            for (float x = startX; x <= endX; x++)
            {
                positions.Add(new Vector3(x, 0, NodeStart.Z));
            }
        }
        return positions;
    }



    /// <summary>
    /// Will be called upon wall destruction. Removes the wall from any connected walls lists of nodes and tells them to delete themselves if that results in them being orphaned.
    /// </summary>
    void OnDestroy()
    {
        if (NodeStart != null)
        {
            NodeStart.connectedWalls.Remove(this);
            NodeStart.DeleteIfOrphaned();
        }

        if (NodeEnd != null)
        {
            NodeEnd.connectedWalls.Remove(this);
            NodeEnd.DeleteIfOrphaned();
        }
    }
}