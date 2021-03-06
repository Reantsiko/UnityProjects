﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
public class Menu : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int sceneBuildIndex = 0;
    [SerializeField] private Image musicImage = null;
    [SerializeField] private Image sfxImage = null;
    [SerializeField] private Sprite[] musicSprites = null;
    [SerializeField] private Sprite[] sfxSprites = null;
    [SerializeField] private AudioSource audioSource = null;
    
    [Header("Main Menu")]
    [SerializeField] private List<GameObject> menus = null;
    [SerializeField] private GameObject quitButton = null;
    [SerializeField] private List<TMP_Text> scoreTexts = null;

    [Header("In Game")]
    [SerializeField] private GameObject gameOverlay = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject gameOverMenu = null;
    [SerializeField] private GameObject recordText = null;
    [SerializeField] private TMP_Text finalScoreText = null;
    [SerializeField] private TMP_Text playerLivesText = null;
    [SerializeField] public bool isPaused = false;

    public void StartGame(int toSet)
    {
        if (toSet == -1)
        {
            Debug.LogWarning($"Improve this so that it doesn't have to reload completely");
            GameManager.instance.ResetSettings(GameManager.instance.difficulty);
        }
        else
            GameManager.instance.ResetSettings((Difficulty)toSet);
        SceneManager.LoadSceneAsync(1);
    }
    
    public void OpenMenu(GameObject toOpen)
    {
        foreach (var menu in menus)
            menu.SetActive(false);
        toOpen.SetActive(true);
    }
    public void QuitGame() => Application.Quit();

    public void SetMusic()
    {
        SoundHandler.instance.ChangeMuteMusic();
        SetSoundImage();
    }

    public void SetSFX()
    {
        SoundHandler.instance.isSoundEffectMuted = !SoundHandler.instance.isSoundEffectMuted;
        SoundHandler.instance.ChangeMuteSoundEffect();
        SetSFXImage();
    }

    private void SetSoundImage()
    {
        int img = SoundHandler.instance.GetIsMuted() == true ? 1 : 0;
        if (musicImage != null && musicSprites != null && musicSprites.Length == 2)
            musicImage.sprite = musicSprites[img];
    }

    private void SetSFXImage()
    {
        int img = SoundHandler.instance.isSoundEffectMuted == true ? 1 : 0;
        if (sfxImage != null && sfxSprites != null && sfxSprites.Length == 2)
            sfxImage.sprite = sfxSprites[img];
    }

    public void LoadMainMenu()
    {
        audioSource?.UnPause();
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    private void Start()
    {
        GameManager.instance.menu = this;
        audioSource = FindObjectOfType<AudioSource>();
        sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        musicImage.sprite = musicSprites[audioSource.mute == true ? 1 : 0];
        if (sceneBuildIndex == 0)
            UpdateScores();
        SoundHandler.instance.ClearSoundEffectSources();
        SetSoundImage();
        SetSFXImage();
#if UNITY_WEBGL
        if (quitButton != null)
            quitButton.SetActive(false);
#endif
    }

    private void Update()
    {
        if (sceneBuildIndex == 0) return;
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.playerLives >= 0)
            PauseGame();
    }

    public void UpdatePlayerLives()
    {
        if (playerLivesText == null) return;
        playerLivesText.text = $"x{(GameManager.instance.playerLives >= 0 ? GameManager.instance.playerLives : 0)}";
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            audioSource?.Pause();
            pauseMenu?.SetActive(isPaused);
        }
        else
        {
            audioSource?.UnPause();
            menus.ForEach(m => m?.SetActive(false));
        }
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void GameOver(int playerScore)
    {
        gameOverlay?.SetActive(false);
        pauseMenu?.SetActive(false);
        gameOverMenu?.SetActive(true);
        recordText?.SetActive(HighScoreManager.instance.CheckPlayerScore(playerScore));
        if (finalScoreText != null)
            finalScoreText.text = playerScore.ToString();
    }

    public void UpdateScores()
    {
        for (int i = 0; i < HighScoreManager.instance.GetScoreAmounts(); i++)
        {
            if (scoreTexts != null && scoreTexts[i] != null)
                scoreTexts[i].text = HighScoreManager.instance.GetScore(i).ToString();
        }
    }
}
