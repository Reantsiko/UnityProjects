using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WaveConfig")]
public class WaveConfiguration : ScriptableObject
{
    [SerializeField] private GameObject[] enemiesPrefab = null;
    [SerializeField] private GameObject pathPrefab = null;
    [SerializeField] private float timeBetweenSpawns = 0.5f;
    [SerializeField] private float spawnRandomFactor = 0.3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int numberOfEnemies = 1;
    [SerializeField] private Difficulty difficulty = Difficulty.Easy;

    private GameObject enemyPrefab;

    public GameObject GetEnemyPrefab() => enemyPrefab;
    public List<Transform> GetWayPoints()
    {
        var waveWayPoints = new List<Transform>();

        foreach (Transform waypoint in pathPrefab.transform)
            waveWayPoints.Add(waypoint);

        return waveWayPoints;
    }

    public void PickEnemy() => enemyPrefab = enemiesPrefab[Random.Range(0, enemiesPrefab.Length)];
    public float GetTimeBetweenSpawns() => timeBetweenSpawns; 
    public float GetSpawnRandomFactor() => spawnRandomFactor; 
    public float GetMoveSpeed() => moveSpeed;
    public int GetNumberOfEnemies() => numberOfEnemies; 
    private void Start() => PickEnemy();
}
