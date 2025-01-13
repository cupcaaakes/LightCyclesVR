using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshRefresh : MonoBehaviour
{
    Unity.AI.Navigation.NavMeshSurface surface;

    void Awake()
    {
        surface = GetComponent<Unity.AI.Navigation.NavMeshSurface>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(BuildNewNavMesh());
    }

    IEnumerator BuildNewNavMesh()
    {
        while (true)
        {
            surface.BuildNavMesh();
            yield return new WaitForSeconds(5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
