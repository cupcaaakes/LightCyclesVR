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
    private Node _nodeStart;

    /// <summary>
    /// former initial node turned permanent node.
    /// </summary>
    public Node NodeStart
    {
        get { return _nodeStart; }
        set
        {
            _nodeStart = value;
        }
    }

    /// <summary>
    /// Private former cursor node turned permanent node.
    /// </summary>
    [SerializeField]
    private Node _nodeEnd;

    /// <summary>
    /// former cursor node turned permanent node.
    /// </summary>
    public Node NodeEnd
    {
        get { return _nodeEnd; }
        set
        {
            _nodeEnd = value;
        }
    }

    public Mesh mesh;

    private static float bottomHeight = -0.05f;


    public float MidX { get; private set; }
    public float MidZ { get; private set; }
    public float MidHeight { get; private set; }


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
        

    }

    /// <summary>
    /// Updates the position of the wall when it is moved.
    /// </summary>
    public void UpdatePos()
    {
        if (!gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            Debug.LogError("ERROR: MeshFilter component missing!");
            return;
        }

        mesh = meshFilter.mesh;

        NodeStart.UpdatePosition();
        NodeEnd.UpdatePosition();

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

        Vector3 worldVertBotLeft, worldVertTopLeft, worldVertBotRight, worldVertTopRight;

        // Set the world positions of the vertices. Not sure why I need to disable a warning here since these are pretty important for self-explanatory reasons.
#pragma warning disable IDE0090
        worldVertBotLeft = new Vector3(NodeStart.X, bottomHeight, NodeStart.Z);                     // Bottom-left vertex
        worldVertTopLeft = new Vector3(NodeStart.X, bottomHeight + NodeStart.Height, NodeStart.Z);  // Top-left vertex
        worldVertBotRight = new Vector3(NodeEnd.X, bottomHeight, NodeEnd.Z);                        // Bottom-right vertex
        worldVertTopRight = new Vector3(NodeEnd.X, bottomHeight + NodeEnd.Height, NodeEnd.Z);      // Top-right vertex

        mesh.vertices = new Vector3[4] { worldVertBotLeft, worldVertTopLeft, worldVertBotRight, worldVertTopRight };

        this.name = WallId + "-" + "Wall" + NodeStart.name + "-/-" + NodeEnd.name;
    }

    public void CreateNewWall(Node nodeStart, Node nodeEnd)
    {
        this._nodeStart = nodeStart;
        this._nodeEnd = nodeEnd;

        UpdatePos();
    }
}