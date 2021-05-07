using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    const float colliderGenerationDistanceTreshold = 5;
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord;
    GameObject meshObject;
    Vector2 sampleCenter;
    Bounds bounds;
    HeightMap mapData;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;
    int previousLODIndex = -1;
    bool hasSetCollider;
    bool heightMapReceived;
    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;
    float maxViewDst;
    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
    {
        this.coord = coord;
        this.viewer = viewer;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;
        meshObject.transform.position = new Vector3(position.x, 0f, position.y);
        meshObject.transform.parent = parent;
        SetVisible(false);
        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshhold;

        
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCenter), OnHeightMapReceived);
    }

    void OnHeightMapReceived(object heightMapObject)
    {
        this.mapData = (HeightMap)heightMapObject;
        heightMapReceived = true;
        UpdateTerrainChunk();
    }

    void OnMeshDataReceived(MeshData meshData)
    {
        meshFilter.mesh = meshData.CreateMesh();
    }

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    public void UpdateTerrainChunk()
    {
        if (!heightMapReceived) return;
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool wasVisible = IsVisible();
        bool visible = viewerDstFromNearestEdge <= maxViewDst;

        if (visible)
        {
            int lodIndex = 0;
            for (int i = 0; i < detailLevels.Length - 1; i++)
            {
                if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshhold)
                {
                    lodIndex = i + 1;
                }
                else
                    break;
            }
            if (lodIndex != previousLODIndex)
            {
                LODMesh lodMesh = lodMeshes[lodIndex];
                if (lodMesh.hasMesh)
                {
                    previousLODIndex = lodIndex;
                    meshFilter.mesh = lodMesh.mesh;
                }
                else if (!lodMesh.hasRequestedMesh)
                {
                    lodMesh.RequestMesh(mapData, meshSettings);
                }
            }


        }
        if (wasVisible != visible)
        {
            SetVisible(visible);
            if (onVisibilityChanged != null)
                onVisibilityChanged(this, visible);
        }

    }
    public void UpdateCollisionMesh()
    {
        if (hasSetCollider) return;
        float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

        if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstTreshold)
            if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
            {
                lodMeshes[colliderLODIndex].RequestMesh(mapData, meshSettings);
            }

        if (sqrDstFromViewerToEdge < colliderGenerationDistanceTreshold * colliderGenerationDistanceTreshold)
        {
            if (lodMeshes[colliderLODIndex].hasMesh)
            {
                meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                hasSetCollider = true;
            }
        }
    }
    public void SetVisible(bool visible) => meshObject.SetActive(visible);
    public bool IsVisible() => meshObject.activeSelf;
}

class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action updateCallback;
    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }
}
[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDstThreshhold;

    public float sqrVisibleDstTreshold
    {
        get
        {
            return visibleDstThreshhold * visibleDstThreshhold;
        }
    }
}