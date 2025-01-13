using System.Collections;
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

    public bool colorBlue;

    public bool isActive;


    public float MidX { get; private set; }
    public float MidZ { get; private set; }
    public float MidHeight { get; private set; }

    public Vector3 worldVertBotLeft, worldVertTopLeft, worldVertBotRight, worldVertTopRight;

    /// <summary>
    /// Pretty self-explanatory. The ID of the wall. You knew that already, come on.
    /// </summary>
    public uint WallId { get; set; }

    private BoxCollider wallCollider;

    /// <summary>
    /// Called immediately upon wall creation, before Start().
    /// </summary>
    void Awake()
    {
        if (!TryGetComponent<BoxCollider>(out wallCollider))
        {
            wallCollider = gameObject.AddComponent<BoxCollider>();
        }
        isActive = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayColliderActivation());
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
        MidX = (NodeStart.X + NodeEnd.X) / 2;

        // Calculate the wall's Z coordinate.
        MidZ = (NodeStart.Z + NodeEnd.Z) / 2;

        // Calculate the wall's height midpoint (within Y coordinate).
        MidHeight = ((NodeStart.Height + NodeEnd.Height) / 2) / 2;

        // Update the wall position to the calculated midpoint
        objectTransform.position = new Vector3(MidX, MidHeight, MidZ);

        UpdateCollider();

        // Calculate world space positions
        Vector3 worldVertBotLeft = new Vector3(NodeStart.X, bottomHeight, NodeStart.Z);
        Vector3 worldVertTopLeft = new Vector3(NodeStart.X, bottomHeight + NodeStart.Height, NodeStart.Z);
        Vector3 worldVertBotRight = new Vector3(NodeEnd.X, bottomHeight, NodeEnd.Z);
        Vector3 worldVertTopRight = new Vector3(NodeEnd.X, bottomHeight + NodeEnd.Height, NodeEnd.Z);

        // Convert to local space
        Vector3 localVertBotLeft = objectTransform.InverseTransformPoint(worldVertBotLeft);
        Vector3 localVertTopLeft = objectTransform.InverseTransformPoint(worldVertTopLeft);
        Vector3 localVertBotRight = objectTransform.InverseTransformPoint(worldVertBotRight);
        Vector3 localVertTopRight = objectTransform.InverseTransformPoint(worldVertTopRight);

        

        // Update the mesh vertices in local space
        mesh.vertices = new Vector3[4] { localVertBotLeft, localVertTopLeft, localVertBotRight, localVertTopRight };


    }

    private void UpdateCollider()
    {
        if (wallCollider == null) return;

        // Calculate the center of the collider
        Vector3 colliderCenter = new Vector3(MidX, MidHeight, MidZ);

        // Calculate dimensions
        float colliderWidth = Vector3.Distance(new Vector3(NodeStart.X, 0, NodeStart.Z), new Vector3(NodeEnd.X, 0, NodeEnd.Z));
        float colliderHeight = Mathf.Abs(NodeStart.Height) + Mathf.Abs(bottomHeight);
        float colliderDepth = 0.1f; // Thin depth for a wall

        // Update the collider size
        wallCollider.size = new Vector3(colliderWidth, colliderHeight, colliderDepth);

        // Update the collider center
        wallCollider.center = transform.InverseTransformPoint(colliderCenter);

        // Calculate the rotation of the collider
        Vector3 direction = new Vector3(NodeEnd.X - NodeStart.X, 0, NodeEnd.Z - NodeStart.Z).normalized;
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        // Apply rotation to the collider by adjusting the object's rotation
        transform.rotation = Quaternion.Euler(0, -angle, 0);
    }

    public void CreateNewWall(Node nodeStart, Node nodeEnd)
    {
        this._nodeStart = nodeStart;
        this._nodeEnd = nodeEnd;

        UpdatePos();
    }

    public void UpdateWall(Node nodeStart, Node nodeEnd)
    {
        this._nodeStart = nodeStart;
        this._nodeEnd = nodeEnd;
    }

    public void NameWall(uint wallId)
    {
        WallId = wallId;
        this.name = wallId + "-" + "Wall" + NodeStart.name + "-/-" + NodeEnd.name;
    }

    private IEnumerator DelayColliderActivation()
    {
        yield return new WaitForSeconds(2f);
        isActive = true;
    }
}