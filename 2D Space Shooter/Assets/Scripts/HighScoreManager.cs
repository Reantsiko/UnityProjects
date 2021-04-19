using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager instance;
    public List<int> highScores = new List<int>();
    [SerializeField] private int scoreAmounts = 5;
    [SerializeField] private int defaultScore = 10000;
    [SerializeField] private List<string> playerPrefKeys;

    private void Awake()
    {
        if (FindObjectsOfType<HighScoreManager>().Length == 1)
        {
            instance = this;
            if (playerPrefKeys == null || playerPrefKeys.Count == 0) return;

            LoadScores();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }

    private void LoadScores()
    {
        for (int i = 0; i < scoreAmounts; i++)
        {
            if (PlayerPrefs.HasKey(playerPrefKeys[i]))
                highScores.Add(PlayerPrefs.GetInt(playerPrefKeys[i]));
            else
            {
                var score = defaultScore * (i + 1);
                highScores.Add(score);
            }
        }
        highScores.Sort();
        highScores.Reverse();
    }
    public int GetScore(int pos)
    {
        if (highScores == null)
            LoadScores();
        return highScores[pos];
    }

    public bool CheckPlayerScore(int score)
    {
        if (highScores != null && highScores.Any(hs => hs <= score))
        {
            for (int i = 0; i < scoreAmounts; i++)
            {
                if (highScores[i] <= score)
                {
                    var temp = highScores[i];
                    highScores[i] = score;
                    score = temp;
                    SaveScore(playerPrefKeys[i], highScores[i]);
                }
            }
            return true;
        }
        return false;
    }

    private void SaveScore(string key, int score) => PlayerPrefs.SetInt(key, score);
    public int GetScoreAmounts() => scoreAmounts;
}
