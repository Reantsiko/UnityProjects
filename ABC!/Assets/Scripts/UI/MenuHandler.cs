using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _menuScreens = null;
    [SerializeField] private bool inGame = false;
    [SerializeField] public bool paused = false;
    [SerializeField] private FirstPersonController _fpc = null;
    [SerializeField] private Slider _mouseSlider = null;
    [SerializeField] private Keybindings keybindings = null;
    [SerializeField] private GameObject _cleanOverlay;
    [SerializeField] private Image _cleanSlider;
    [SerializeField] private GameObject gameOverlayUI;
    [SerializeField] private GameObject backgroundImage;
    [SerializeField] private GameObject _countdownScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text _floorText;
    [SerializeField] private CheckFloorTouch _checkFloorTouch;
    [SerializeField] private GameObject returnConfirmation;
    [SerializeField] private LevelHandler _levelHandler;
    //[SerializeField] private int current = 0;
    private string lastOpened = null;
    private void Start()
    {
        if (!_levelHandler)
            _levelHandler = FindObjectOfType<LevelHandler>();
        if (inGame)
        {
            if (!_fpc)
                _fpc = FindObjectOfType<FirstPersonController>();
            if (!_checkFloorTouch)
                _checkFloorTouch = FindObjectOfType<CheckFloorTouch>();
            OpenMenu("In Game Overlay");
        }
        else
        {
            OpenMenu("Main Menu");
        }
        ProgressTracker.LoadData();
        keybindings.FillDictionary();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ProgressTracker.LoadData();
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ProgressTracker.SaveData();
#if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ProgressTracker.PrintData();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ProgressTracker.IncreaseLevelTest();
#endif
#endif
        if (!inGame || _checkFloorTouch.GetGameOver()) return;
        if (Input.GetKeyDown(keybindings.GetKeyCode(KeybindEnum.menu)))
            PauseUnpause();
    }

    public void PauseUnpause()
    {
        if (returnConfirmation)
            returnConfirmation.SetActive(false);
        paused = !paused;
        backgroundImage.SetActive(paused);
        if (paused)
        {
            Time.timeScale = 0;
            OpenMenu("Pause Menu");
            _mouseSlider.value = keybindings.GetMouseSensitivity();
        }
        else
        {
            OpenMenu("In Game Overlay");
            Time.timeScale = 1;
            _fpc.m_MouseLook.XSensitivity = keybindings.GetMouseSensitivity();
            _fpc.m_MouseLook.YSensitivity = keybindings.GetMouseSensitivity();
        }
        _fpc.enabled = !paused;
        _fpc.m_MouseLook.SetCursorLock(!paused);
    }

    public void RestartLevel()
    {
        _levelHandler.RestartLevel();
    }

    public void LoadNextLevel()
    {
        _levelHandler.LoadNextLevel();
    }

    public void OpenMenu(string name)
    {
        foreach(var menu in _menuScreens)
        {
            if (menu.activeSelf)
                lastOpened = menu.name;
            if (name == menu.name)
                menu.SetActive(true);
            else
                menu.SetActive(false);
        }
    }

    public GameObject GetGameOverlay() { return gameOverlayUI; }

    public void OpenMenu(GameObject obj)
    {
        foreach (var menu in _menuScreens)
        {
            if (menu.activeSelf)
                lastOpened = menu.name;
            if (menu != obj)
                menu.SetActive(false);
        }
        obj.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartLevel(int index)
    {
        Time.timeScale = 1;
        _levelHandler.LoadLevel(index);
    }

    public void ReturnConfirmation()
    {
        _menuScreens[0].SetActive(false);
        returnConfirmation.SetActive(true);
    }

    public void CancelReturnConfirmation()
    {
        returnConfirmation.SetActive(false);
        _menuScreens[0].SetActive(true);
    }

    public void OpenMainMenu()
    {
        _levelHandler.LoadLevel(0);
    }

    public void GoBack()
    {
        OpenMenu(lastOpened);
    }

    public void SaveMouseSens()
    {
        keybindings.SetMouseSensitivity(_mouseSlider.value);
        _fpc.m_MouseLook.XSensitivity = keybindings.GetMouseSensitivity();
        _fpc.m_MouseLook.YSensitivity = keybindings.GetMouseSensitivity();
        PlayerPrefs.Save();
    }

    public void OpenEndLevelScreen(bool victory, string endLevelMessage) //VERANDEREN
    {
        var gameOver = gameOverScreen.GetComponent<LevelFinishedUI>();
        if (gameOver == null)
            return;
        //gameOverlayUI.SetActive(false);
        //OpenMenu(gameOverScreen);
        gameOver.SetLevelFinishedScreen(victory, endLevelMessage);
    }

    public void ActivateCountdown(bool toSet)
    {
        if (_countdownScreen)
            _countdownScreen.SetActive(toSet);
    }

    public void CountdownMessage(int timeLeft)
    {
        if (_floorText)
            _floorText.text = "The floor will become lava in: " + timeLeft;
    }

    public GameObject GetCleanOverlay() { return _cleanOverlay; }
    public void ActivateCleanOverlay(bool activate)
    {
        if (!_cleanOverlay || !_cleanSlider) return;
        _cleanOverlay.SetActive(activate);
    }

    public void FillCleanedAmount(float amount)
    {
        if (!_cleanOverlay || !_cleanSlider) return;
        _cleanSlider.fillAmount = amount;
    }
}
