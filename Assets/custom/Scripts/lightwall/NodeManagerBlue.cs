using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class NodeManagerBlue : MonoBehaviour
{
    public bool InitNodesSet { get; private set; }

    [SerializeField]
    private Transform PlacedNodes;

    public List<Node> Nodes { get; private set; }

    public uint NodeCounter { get; private set; }

    public GameObject nodePrefab;

    public GameObject Cycle;

    public static NodeManagerBlue Instance { get; private set; }

    public Node DragNode
    {
        get { return _dragNode; }
        set
        {
            _dragNode = value;
        }
    }

    private Node _dragNode;

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
        // Initialize DragNode at the first node's position
        (_dragNode, _) = InstantiateNode(Cycle.transform.position.x, Cycle.transform.position.z);
        _dragNode.name = "DragNodeBlue";

        // Instantiate the first node at the origin
        //var (node, _) = InstantiateNode(0, 0);
    }

    void Update()
    {
        if (Cycle == null)
        {
            Debug.LogWarning("Cycle is not assigned. Please assign Cycle to update DragNode position.");
            return;
        }

        // Update DragNode's position to match Cycle's position
        _dragNode.transform.position = Cycle.transform.position;
        _dragNode.GetComponent<Node>().UpdatePosition(Cycle.transform.position.x, Cycle.transform.position.z);
    }

    public (Node, GameObject) InstantiateNode(float x, float z)
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

    public IEnumerator PlaceNodesAndCreateWalls()
    {
        while (true)
        {
            // Place a new node
            var (newNode, _) = InstantiateNode(DragNode.transform.position.x, DragNode.transform.position.z);
            /*
            Wall oldWall;

            if (WallManager.Instance.walls.Count > 0)
            {
                oldWall = WallManager.Instance.walls[^1];
                oldWall.UpdateWall(newNode, oldWall.NodeStart);
            }
            
            // Create a wall between DragNode and the new node
            Wall newWall = WallManager.Instance.CreateWall(DragNode, newNode);
            WallManager.Instance.walls.Add(newWall);*/
            // Wait for half a second#
            if (Cycle.GetComponent<Cycle>().DeadZoneActive)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }


        }
    }

    /*
    public void FinalizeDragNode()
    {
        if (_dragNode == null)
        {
            Debug.LogError("DragNode is null. Cannot finalize.");
            return;
        }

        // Finalize the DragNode as a permanent node
        Node dragNode = _dragNode.GetComponent<Node>();
        if (dragNode != null)
        {
            if (!Nodes.Contains(dragNode))
            {
                Nodes.Add(dragNode);
            }
        }

        // Create a new DragNode for the next wall
        _dragNode = Instantiate(nodePrefab, _dragNode.transform.position, Quaternion.identity, PlacedNodes.transform).transform;
        _dragNode.name = "DragNode";
    }*/
}
