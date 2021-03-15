using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public GameObject moveWordPrefab;
    public Transform canvas;
    public Transform worldCanvas;
    public WordDisplay SpawnWord(bool isMovement, Vector3 spawnPos)
    {
        Vector3 randomPos = new Vector3(Random.Range(-8f, 8f), 7f, 0f);
        GameObject wordObj;
        if (!isMovement)
            wordObj = Instantiate(wordPrefab, randomPos, Quaternion.identity, canvas);
        else
            wordObj = Instantiate(moveWordPrefab, spawnPos, Quaternion.identity, worldCanvas);

        var wordDisplay = wordObj.GetComponent<WordDisplay>();
        return wordDisplay;
    }
}
