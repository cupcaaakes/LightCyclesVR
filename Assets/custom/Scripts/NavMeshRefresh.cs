using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshRefresh : MonoBehaviour
{
    public NavMeshSurface surface;
    public float rebakeInterval = 5f;

    private NavMeshData currentNavMeshData;
    private NavMeshDataInstance currentNavMeshInstance;

    void Start()
    {
        if (surface == null)
        {
            Debug.LogError("NavMeshSurface is not assigned! Please assign it in the Inspector.");
            return;
        }

        InvokeRepeating(nameof(RebakeNavMesh), 0f, rebakeInterval);
    }

    private void RebakeNavMesh()
    {
        if (surface != null)
        {
            // Remove the previous NavMesh instance
            if (currentNavMeshInstance.valid)
            {
                currentNavMeshInstance.Remove();
            }

            // Filter objects to exclude those with NavMeshModifier and the "Collidable" tag
            var sources = GetFilteredSources();

            // Build NavMesh data with the filtered sources
            var bounds = surface.navMeshData != null ? surface.navMeshData.sourceBounds : new Bounds(Vector3.zero, Vector3.one * 5000f);
            currentNavMeshData = NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(), sources, bounds, surface.transform.position, surface.transform.rotation);

            if (currentNavMeshData != null)
            {
                currentNavMeshInstance = NavMesh.AddNavMeshData(currentNavMeshData);
                Debug.Log("NavMesh rebaked at: " + Time.time);
            }
            else
            {
                Debug.LogWarning("NavMesh data could not be built.");
            }
        }
    }

    private List<NavMeshBuildSource> GetFilteredSources()
    {
        var sources = new List<NavMeshBuildSource>();
        var modifiers = FindObjectsOfType<NavMeshModifier>();

        foreach (var modifier in modifiers)
        {
            if (modifier.gameObject.CompareTag("Collidable"))
            {
                // Skip objects with the "Collidable" tag
                continue;
            }

            var meshFilter = modifier.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                var source = new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.Mesh,
                    sourceObject = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix,
                    area = modifier.overrideArea ? modifier.area : 0
                };

                sources.Add(source);
            }
        }

        return sources;
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(RebakeNavMesh));

        // Cleanup the current NavMesh when the script is destroyed
        if (currentNavMeshInstance.valid)
        {
            currentNavMeshInstance.Remove();
        }
    }
}
