using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class WordLists : MonoBehaviour
{
    public static WordLists instance = null;
    public Dictionary<Difficulty, List<string>> wordLists;
    
    private void InitializeLists()
    {
        wordLists = new Dictionary<Difficulty, List<string>>();
        foreach (var diff in Enum.GetValues(typeof(Difficulty)))
        {
            wordLists.Add((Difficulty)diff.GetHashCode(), new List<string>());

            var path = Path.Combine(Application.streamingAssetsPath, diff.ToString());
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = string.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        wordLists[(Difficulty)diff.GetHashCode()].Add(s);
                    }
                }
            }
            else
                Debug.LogError($"File not found with path: {path}");
        }
        
    }

    public string GetRandomWord()
    {
        int modifier = GameManager.instance.difficulty.GetHashCode() > 4 ? 2 : 0;
        var diff = UnityEngine.Random.Range(1, GameManager.instance.difficulty.GetHashCode() + 1);
        diff = (diff + modifier) <= GameManager.instance.difficulty.GetHashCode() ? (diff + modifier) : diff;
        var list = wordLists[(Difficulty)diff];
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        InitializeLists();
    }
}
