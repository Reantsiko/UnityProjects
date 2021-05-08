using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTerrain : MonoBehaviour
{
    public int width = 10;
    public int depth = 10;
    public GameObject terrainPrefab;
    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                var spawnPos = new Vector3(x + .5f, Random.Range(-2f, 2f), z + .5f);
                Instantiate(terrainPrefab, spawnPos, Quaternion.identity, transform);
            }
        }
        PathFindingTest.instance.StartGridBuilding();
    }
}
