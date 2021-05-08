using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class MeshSettings : UpdateableData
{
    public const int numSupportedLODs = 5;
    public const int numSupportChunkSizes = 9;
    public const int numSupportFlatShadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float meshScale = 5f;
    public bool useFlatShading;
    [Range(0, numSupportChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numSupportFlatShadedChunkSizes - 1)]
    public int flatShadedChunkSizeIndex;

    //num verts per line of mesh rendered at LOD = 0.
    //Includes the 2 extra vertices that are excluded from final mesh, but used for calulating normals.
    public int numVertsPerLine
    {
        get => supportedChunkSizes[useFlatShading ? flatShadedChunkSizeIndex : chunkSizeIndex] + 5;
    }

    public float meshWorldSize
    {
        get => (numVertsPerLine - 3) * meshScale;
    }
}
