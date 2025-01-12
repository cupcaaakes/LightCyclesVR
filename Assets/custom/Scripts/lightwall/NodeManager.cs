using System.Collections.Generic;
using System;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public bool InitNodesSet { get; private set; }

    [SerializeField]
    private Transform PlacedNodes;

    [SerializeField]
    public Transform DragNode { get; private set; }

    public event Action<Node, Node> WallCreated;

    public List<Node> Nodes { get; private set; }

    public uint NodeCounter { get; private set; }

    public GameObject nodePrefab;

    public GameObject Cycle;

    public static NodeManager Instance { get; private set; }

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
        InitNodesSet = false;
        NodeCounter = 0;
        Nodes = new List<Node>();
    }

    void Start()
    {
        // Instantiate the first node at the origin
        var (node, _) = InstantiateNode(0, 0);

        // Initialize DragNode at the first node's position
        DragNode = Instantiate(nodePrefab, node.transform.position, Quaternion.identity, PlacedNodes.transform).transform;
        DragNode.name = "DragNode";
    }

    void Update()
    {
        if (Cycle == null)
        {
            Debug.LogWarning("Cycle is not assigned. Please assign Cycle to update DragNode position.");
            return;
        }

        // Update DragNode's position to match Cycle's position
        DragNode.transform.position = Cycle.transform.position;
    }

    public (Node, GameObject) InstantiateNode(int x, int z)
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Node prefab is not assigned in NodeManager!");
            return (null, null);
        }

        GameObject newNodeObj = Instantiate(nodePrefab, new Vector3(x, 0, z), Quaternion.identity, PlacedNodes.transform);
        newNodeObj.name = "Node" + newNodeObj.transform.position;

        Node newNode = newNodeObj.GetComponent<Node>();
        if (newNode == null)
        {
            Debug.LogError("Node prefab does not have a Node script attached!");
            return (null, null);
        }

        newNode.X = x;
        newNode.Z = z;

        NodeCounter++;
        Nodes.Add(newNode);

        return (newNode, newNodeObj);
    }

    public void FinalizeDragNode()
    {
        if (DragNode == null)
        {
            Debug.LogError("DragNode is null. Cannot finalize.");
            return;
        }

        // Finalize the DragNode as a permanent node
        Node dragNode = DragNode.GetComponent<Node>();
        if (dragNode != null)
        {
            if (!Nodes.Contains(dragNode))
            {
                Nodes.Add(dragNode);
            }

            // Notify WallManager to finalize the temporary wall
            WallManager.Instance?.FinalizeTempWall();
        }

        // Create a new DragNode for the next wall
        DragNode = Instantiate(nodePrefab, DragNode.transform.position, Quaternion.identity, PlacedNodes.transform).transform;
        DragNode.name = "DragNode";
    }
}
