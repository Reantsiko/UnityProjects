using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] GameObject enemySpawner;
	[SerializeField] List<WaveConfiguration> waveConfigs;
	[SerializeField] int startingWave = 0;
	[SerializeField] bool looping = false;
	int currentWave;

	IEnumerator Start()
	{
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
		while (spawnedEnemies < waveConfig.GetNumberOfEnemies())
		{
			var newEnemy = Instantiate(waveConfig.GetEnemyPrefab(),
						waveConfig.GetWayPoints()[startingWave].transform.position,
						Quaternion.identity);
			newEnemy.transform.parent = enemySpawner.transform;
			spawnedEnemies++;
			newEnemy.GetComponent<EnemyPathing>().SetWaveConfiguration(waveConfig);
			yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
		}
	}
}