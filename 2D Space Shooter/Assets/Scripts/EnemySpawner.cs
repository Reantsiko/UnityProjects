using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] private float waitTimeBetweenWaves = 0.5f;
	[SerializeField] GameObject enemySpawner;
	[SerializeField] List<WaveConfiguration> waveConfigs;
	[SerializeField] int startingWave = 0;
	[SerializeField] bool looping = false;
	int currentWave;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(waitTimeBetweenWaves);
		do
		{
			yield return StartCoroutine(SpawnAllWaves());
		} while (looping);
	}

	IEnumerator SpawnAllWaves()
	{
		for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
		{
			var waveToSpawn = waveConfigs[waveIndex];
			yield return StartCoroutine(SpawnAllEnemiesInWave(waveToSpawn));
		}
		yield return new WaitForSeconds(waitTimeBetweenWaves);
	}

	IEnumerator SpawnRandomWave()
	{
		int wave = currentWave;
		while (wave == currentWave)
			wave = Random.Range(0, waveConfigs.Count);
		var waveToSpawn = waveConfigs[wave];
		yield return StartCoroutine(SpawnAllEnemiesInWave(waveToSpawn));
	}

	IEnumerator SpawnAllEnemiesInWave(WaveConfiguration waveConfig)
	{
		int spawnedEnemies = 0;
		waveConfig.PickEnemy();
		var pathing = waveConfig.GetWayPoints();
		while (spawnedEnemies < waveConfig.GetNumberOfEnemies())
		{
			var newEnemy = Instantiate(waveConfig.GetEnemyPrefab(),
						pathing[0].position,
						Quaternion.identity);
			newEnemy.transform.parent = enemySpawner.transform;
			spawnedEnemies++;
			newEnemy.GetComponent<EnemyPathing>().SetWaveConfiguration(waveConfig, pathing);
			yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
		}
	}
}