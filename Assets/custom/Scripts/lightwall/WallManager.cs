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
    public Wall tempWall;

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
        walls.Clear();
    }

    private void Start()
    {
        StartCoroutine(NodeManager.Instance.PlaceNodesAndCreateWalls());
        /*
        tempWall = CreateWall(NodeManager.Instance.Nodes[^1], NodeManager.Instance.DragNode.GetComponent<Node>());
        walls.Add(tempWall);
        */
    }

    private void Update()
    {
        UpdateWall(walls[^1]);
    }

    public Wall CreateWall(Node startNode, Node endNode)
    {
        // Create a new wall using the wall model
        GameObject wallObject = Instantiate(wallModel, Vector3.zero, Quaternion.identity, transform);
        wallObject.GetComponent<Wall>().CreateNewWall(startNode, endNode);
        return wallObject.GetComponent<Wall>();
    }

    private void UpdateWall(Wall wall)
    {
        wall.GetComponent<Wall>().UpdatePos();
    }

}
