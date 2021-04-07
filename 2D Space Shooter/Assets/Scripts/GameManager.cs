using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Transform playerTransform = null;
    [SerializeField] public Difficulty difficulty = Difficulty.Easy;
    [SerializeField] private TMP_Text scoreText = null;
    [SerializeField] private int playerScore;

    private void Awake()
    {
        instance = this;
        playerScore = 0;
    }

    public void UpdateScore(int scoreChange)
    {
        playerScore += scoreChange;
        if (scoreText != null)
            scoreText.text = playerScore.ToString();
    }
}
