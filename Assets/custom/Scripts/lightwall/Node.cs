using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A node is a 2 dimensional position within the game world. It serves as end points for the walls to connect to. Contains an X value, a Z value and a height value, which is basically a line going from Y= 0 to the designated height value.
/// </summary>
public class Node : MonoBehaviour//, IComparable<Node>
{
    /// <summary>
    /// Private X value of the node.
    /// </summary>
    [SerializeField]
    private float _x;

    /// <summary>
    /// X value of the node.
    /// </summary>
    public float X
    {
        get { return _x; }
        set
        {
            _x = value;
            UpdatePosition();
        }
    }

    /// <summary>
    /// Private Z value of the node.
    /// </summary>
    [SerializeField]
    private float _z;

    /// <summary>
    /// Z value of the node.
    /// </summary>
    public float Z
    {
        get { return _z; }
        set
        {
            _z = value;
            UpdatePosition();
        }
    }

    /// <summary>
    /// Private height of the node.
    /// </summary>
    [SerializeField]
    private int _height;

    /// <summary>
    /// Height of the node.
    /// </summary>
    public int Height
    {
        get { return _height; }
        set
        {
            _height = value;
        }
    }

    /// <summary>
    /// List of all walls the node is connected to.
    /// </summary>
    public List<Wall> connectedWalls = new();

    // Start is called before the first frame update
    void Start()
    {
        UpdatePosition();
        _height = 3; // Default number is now the magic number 3, sue me
    }

    /// <summary>
    /// Update the position of the node.
    /// </summary>
    public void UpdatePosition()
    {
        if (this != null) transform.position = new Vector3(_x, 0, _z);
    }

    /// <summary>
    /// Deletes the node if there are no walls connected.
    /// </summary>
    public void DeleteIfOrphaned()
    {
        if (connectedWalls.Count == 0)
        {
            Destroy(gameObject);
        }
    }
    /*
    public int CompareTo(Node other)
    {
        if (other == null) return 1;
        return X.CompareTo(other.X);
    }

    public int CompareToByZ(Node other)
    {
        if (other == null) return 1;
        return Z.CompareTo(other.Z);
    }
    */
}