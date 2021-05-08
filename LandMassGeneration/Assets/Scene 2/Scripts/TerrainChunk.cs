using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public TerrainChunk(Vector2 _coord, HeightMapSettings _heightMapSettings, MeshSettings _meshSettings, LODInfo[] _detailLevels, int _colliderLODIndex, Transform _parent, Transform _viewer, Material _material)
    {
        coord = _coord;
        viewer = _viewer;
        detailLevels = _detailLevels;
        colliderLODIndex = _colliderLODIndex;
        heightMapSettings = _heightMapSettings;
        meshSettings = _meshSettings;
        sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = _material;
        meshObject.transform.position = new Vector3(position.x, 0f, position.y);
        meshObject.transform.parent = _parent;
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
        mapData = (HeightMap)heightMapObject;
        heightMapReceived = true;
        UpdateTerrainChunk();
    }

    void OnMeshDataReceived(MeshData meshData)
    {
        meshFilter.mesh = meshData.CreateMesh();
    }

    Vector2 viewerPosition
    {
        get => new Vector2(viewer.position.x, viewer.position.z);
    }

    public void UpdateTerrainChunk()
    {
        if (!heightMapReceived) return;
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool wasVisible = IsVisible();
        bool visible = viewerDstFromNearestEdge <= maxViewDst;

        if (visible)
            UpdateVisibleTerrain(viewerDstFromNearestEdge);
        if (wasVisible != visible)
        {
            SetVisible(visible);
            if (onVisibilityChanged != null)
                onVisibilityChanged(this, visible);
        }
    }

    private void UpdateVisibleTerrain(float viewerDstFromNearestEdge)
    {
        int lodIndex = 0;
        for (int i = 0; i < detailLevels.Length - 1; i++)
        {
            if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshhold)
                lodIndex = i + 1;
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
                lodMesh.RequestMesh(mapData, meshSettings);
        }
    }
    public void UpdateCollisionMesh()
    {
        if (hasSetCollider) return;
        float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

        if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstTreshold &&
            !lodMeshes[colliderLODIndex].hasRequestedMesh)
                lodMeshes[colliderLODIndex].RequestMesh(mapData, meshSettings);

        if (sqrDstFromViewerToEdge < colliderGenerationDistanceTreshold * colliderGenerationDistanceTreshold &&
            lodMeshes[colliderLODIndex].hasMesh)
        {
                meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                hasSetCollider = true;
        }
    }
    public void SetVisible(bool visible) => meshObject.SetActive(visible);
    public bool IsVisible() => meshObject.activeSelf;
}