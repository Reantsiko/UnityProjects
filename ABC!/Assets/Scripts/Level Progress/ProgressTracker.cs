using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ProgressTracker
{
#if UNITY_STANDALONE
    static private SaveData progress;
    static public Difficulty difficulty = Difficulty.normal;
    public static void SaveData()
    {
        if (progress == null)
            InitializeArrays();
        if (SceneManager.sceneCountInBuildSettings - 1 != progress.LevelProgress.Length)
            UpdateData();
        ProgressSaver.SaveProgressData(progress);
    }

    public static void LoadData()
    {
        progress = ProgressSaver.LoadProgressData();
        if (progress == null)
        {
            SaveData();
            return;
        }
        
        progress.InputLoaded(progress);
    }

     public static SaveData GetSaveData() { return progress; }
    public static void LevelCompleted(int level)
    {
        if (progress == null)
            InitializeArrays();
        progress.LevelCompleted(level);
    }

    public static bool CheckForRecord(int level, bool time, float score)
    {
        if (progress == null)
            LoadData();
        if (time)
        {
            var temp = progress.BestTimesDifficulty[difficulty][level];
            Debug.Log("Temp time = " + temp);
            if (temp == 0 || score < temp)
            {
                progress.BestTimesDifficulty[difficulty][level] = score;
                return true;
            }
        }
        else
        {
            var temp = progress.BestMessesDifficulty[difficulty][level];
            Debug.Log("Temp mess = " + temp);
            if (temp == 0 || score < temp)
            {
                progress.BestMessesDifficulty[difficulty][level] = score;
                return true;
            }
        }
        return false;
    }

    private static void InitializeArrays()
    {
        progress = new SaveData();
        var levelAm = SceneManager.sceneCountInBuildSettings - 1;
        progress.LevelProgress = InitArr<int>(levelAm);
        progress.BestTimesDifficulty = InitDictionaries(levelAm);
        progress.BestMessesDifficulty = InitDictionaries(levelAm);
    }

    private static void UpdateData()
    {
        var temp = new SaveData();
        var levelAm = SceneManager.sceneCountInBuildSettings - 1;

        temp.LevelProgress = UpdateArr<int>(InitArr<int>(levelAm), progress.LevelProgress);
        temp.BestMessesDifficulty = UpdateDictionaries(levelAm, progress.BestMessesDifficulty);
        temp.BestTimesDifficulty = UpdateDictionaries(levelAm, progress.BestTimesDifficulty);

        progress = temp;
    }

    private static Dictionary<Difficulty, float[]> UpdateDictionaries(int levelAm, Dictionary<Difficulty, float[]> oldArr)
    {
        var temp = new Dictionary<Difficulty, float[]>();
        for (int i = 0; i < Enum.GetNames(typeof(Difficulty)).Length; i++)
            temp[(Difficulty)i] = UpdateArr(InitArr<float>(levelAm), oldArr[(Difficulty)i]);
        return temp;
    }

    public static void PrintData()
    {
        Debug.Log(difficulty);
        for (int i = 0; i < progress.LevelProgress.Length; i++)
            Debug.Log("Level " + i + " = " + progress.LevelProgress[i]);
        if (progress == null) return;
        for (int j = 0; j < Enum.GetNames(typeof(Difficulty)).Length; j++)
        {
            for (int i = 0; i < progress.BestTimesDifficulty[(Difficulty)j].Length; i++)
            {
                Debug.Log((Difficulty)j + " level " + i + " best time: " + progress.BestTimesDifficulty[(Difficulty)j][i]);
                Debug.Log((Difficulty)j + " level " + i + " best mess: " + progress.BestMessesDifficulty[(Difficulty)j][i]);
            }
        }
    }

    public static void IncreaseLevelTest()
    {
        if (progress == null) return;
        //int i = 0;
        for (int x = 0; x < progress.LevelProgress.Length; x++)
        {
            progress.LevelCompleted(x);
        }
        /*for (int j = 0; j < Enum.GetNames(typeof(Difficulty)).Length; j++)
        {
            if (i == SceneManager.sceneCountInBuildSettings - 1)
                i = 0;
            progress.BestTimesDifficulty[(Difficulty)j][i] = 1;
            i++;
        }*/
    }

    private static Dictionary<Difficulty, float[]> InitDictionaries(int levelAm)
    {
        var temp = new Dictionary<Difficulty, float[]>();
        temp.Add(Difficulty.easy, InitArr<float>(levelAm));
        temp.Add(Difficulty.normal, InitArr<float>(levelAm));
        temp.Add(Difficulty.hard, InitArr<float>(levelAm));
        temp.Add(Difficulty.impossible, InitArr<float>(levelAm));
        return temp;
    }
#endif

#if UNITY_WEBGL
    static public Difficulty difficulty = Difficulty.easy;
    public static float[] times;
    public static float[] messes;
    public static int[] finished;

    public static void SaveData() // aanpssen
    {
        PlayerPrefs.SetString("SaveData", "Saved");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            PlayerPrefs.SetFloat("Time " + i, times[i]);
            PlayerPrefs.SetFloat("Mess " + i, messes[i]);
            PlayerPrefs.SetInt("LevelClear " + i, finished[i]);
        }
        PlayerPrefs.Save();
    }

    public static void LoadData() //aanpassen
    {
        InitArrays();
        if (!PlayerPrefs.HasKey("SaveData"))
        {
            SaveData();
            return;
        }
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            times[i] = PlayerPrefs.GetFloat("Time " + i);
            messes[i] = PlayerPrefs.GetFloat("Mess " + i);
            finished[i] = PlayerPrefs.GetInt("LevelClear " + i);
        }
    }

    private static void InitArrays()
    {
        var levelAm = SceneManager.sceneCountInBuildSettings - 1;
        Debug.Log($"Level Amount {levelAm}");
        times = InitArr<float>(levelAm);
        messes = InitArr<float>(levelAm);
        finished = InitArr<int>(levelAm);
    }
#endif

    private static T[] InitArr<T>(int levelAm)
    {
        return new T[levelAm];
    }

    private static T[] UpdateArr<T>(T[] newArr, T[] oldArr)
    {
        for (int i = 0; i < newArr.Length && i < oldArr.Length; i++)
            newArr[i] = oldArr[i];
        return newArr;
    }

    
}
