using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGridInfo : MonoBehaviour
{
    public int xSize, ySize, detail;
    public float size = 0.5f;
    public float waitTime = 0f;
    private Vector3[] vertices;

    private void Awake()
    {
        StartCoroutine(Generate());
    }
    private IEnumerator Generate()
    {
        var mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize * detail + 1) * (ySize * detail + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0, y = 0; y <= ySize * detail; y++)
        {
            for (int x = 0; x <= xSize * detail; x++, i++)
            {
                vertices[i] = new Vector3(x * size, 0f, y * size);
                uv[i] = new Vector2(x / (xSize * detail), y / (ySize * detail));
                yield return new WaitForSeconds(waitTime);
            }
        }
        mesh.vertices = vertices;
        int[] triangles = new int[(xSize * ySize) * (int)Mathf.Pow(detail, 2) * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize * detail; y++, vi++)
        {
            for (int x = 0; x < xSize * detail; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize * detail + 1;
                triangles[ti + 5] = vi + xSize * detail + 2;
                yield return new WaitForSeconds(waitTime);
            }
        }

        mesh.triangles = triangles;
        mesh.uv = uv;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
            Gizmos.DrawSphere(vertices[i], .1f);
    }
}
