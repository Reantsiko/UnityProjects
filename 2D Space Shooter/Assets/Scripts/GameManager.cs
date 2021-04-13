using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Transform playerTransform = null;
    public int playerLives = 3;
    [SerializeField] public Difficulty difficulty = Difficulty.Easy;
    [SerializeField] private TMP_Text scoreText = null;
    
    [Header("Difficulty changer")]
    [SerializeField] private int easyScore = 1000;
    [SerializeField] private int normalScore = 2000;
    [SerializeField] private int hardScore = 3000;
    [SerializeField] private int veryHardScore = 4000;
    [SerializeField] private int impossibleScore = 5000;

    private int playerScore;
    private bool respawning = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        playerScore = 0;
    }

    public void UpdateScore(int scoreChange)
    {
        playerScore += scoreChange;
        var scoreAsText = playerScore.ToString();
        if (scoreText != null)
        {
            if (scoreAsText.Length < 13)
                scoreText.text = scoreAsText.PadLeft(13, '0');
            else
                scoreText.text = scoreAsText;
        }
        DifficultyIncrease();
    }

    private void DifficultyIncrease()
    {
        switch(difficulty)
        {
            case Difficulty.VeryEasy:
                difficulty = playerScore >= easyScore ? Difficulty.Easy : difficulty;
                break;
            case Difficulty.Easy:
                difficulty = playerScore >= normalScore ? Difficulty.Normal : difficulty;
                break;
            case Difficulty.Normal:
                difficulty = playerScore >= hardScore ? Difficulty.Hard : difficulty;
                break;
            case Difficulty.Hard:
                difficulty = playerScore >= veryHardScore ? Difficulty.VeryHard : difficulty;
                break;
            case Difficulty.VeryHard:
                difficulty = playerScore >= impossibleScore ? Difficulty.Impossible : difficulty;
                break;
            default:
                break;
        }
    }

    public void ResetSettings(Difficulty toSet)
    {
        playerScore = 0;
        difficulty = toSet;
        playerLives = 3;
    }

    public void SetScoreText(TMP_Text toSet) => scoreText = toSet;
    public void SetRespawning(bool toSet) => respawning = toSet;
    public bool GetRespawning() => respawning;
}
