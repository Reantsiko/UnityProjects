using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressSaver : MonoBehaviour
{
    public int levels;
    public float[] times;
    public int[] finished;
    public static LevelProgressSaver instance;
    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<LevelProgressSaver>().Length > 1)
            Destroy(this);
        else
            instance = this;
        levels = SceneManager.sceneCountInBuildSettings;
        times = new float[levels];
        finished = new int[levels];
        LoadTimes();
        //print(times[1]);
    }

    public void SaveProgress()
    {
        for (int i = 0; i < levels; i++)
        {
            PlayerPrefs.SetFloat("Level " + i + 1, times[i]);
            PlayerPrefs.SetInt("LevelClear " + i + 1, finished[i]);
        }
    }

    public void LoadTimes()
    {
        for (int i = 0; i < levels; i++)
        {
            times[i] = PlayerPrefs.GetFloat("Level " + i + 1, 999f);
            finished[i] = PlayerPrefs.GetInt("LevelClear " + i + 1, 0);
        }
    }
}
