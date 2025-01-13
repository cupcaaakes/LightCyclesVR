using System.Collections.Generic;
using UnityEngine;

public class WallManagerOrange : MonoBehaviour
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
    public Wall tempWall;

    public static WallManagerOrange Instance { get; private set; }

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
        walls.Clear();
    }

    private void Start()
    {
        //StartCoroutine(NodeManagerOrange.Instance.PlaceNodesAndCreateWalls());
        /*
        tempWall = CreateWall(NodeManager.Instance.Nodes[^1], NodeManager.Instance.DragNode.GetComponent<Node>());
        walls.Add(tempWall);
        */
    }

    private void Update()
    {
        //UpdateWall(walls[^1]);
        if (NodeManagerOrange.Instance.Nodes.Count > 2 && NodeManagerOrange.Instance.Nodes[^2].isConnected == false)
        {
            Wall newWall = CreateWall(NodeManagerOrange.Instance.Nodes[^2], NodeManagerOrange.Instance.Nodes[^1]);
            NodeManagerOrange.Instance.Nodes[^2].isConnected = true;
            newWall.colorBlue = false;
            walls.Add(newWall);
        }
    }

    public Wall CreateWall(Node startNode, Node endNode)
    {
        // Create a new wall using the wall model
        GameObject wallObject = Instantiate(wallModel, Vector3.zero, Quaternion.identity, transform);
        wallObject.GetComponent<Wall>().CreateNewWall(startNode, endNode);
        wallObject.GetComponent<Wall>().NameWall((uint)walls.Count);
        return wallObject.GetComponent<Wall>();
    }

    private void UpdateWall(Wall wall)
    {
        wall.GetComponent<Wall>().UpdatePos();
    }

}
