using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public Transform[] spawnPositions;
    public float timeBetweenSpawns;

    private void Start()
    {
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        while (true)
        {
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            var spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
            Instantiate(prefab, spawnPos.position, Quaternion.Euler(0, -180,0));
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
