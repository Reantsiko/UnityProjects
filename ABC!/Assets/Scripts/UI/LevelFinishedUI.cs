using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class LevelFinishedUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject timeParent;
    [SerializeField] private GameObject messParent;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text timeRecord;
    [SerializeField] private TMP_Text messText;
    [SerializeField] private TMP_Text messRecord;
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private FirstPersonController _fpc;
    [SerializeField] private MenuHandler menuHandler;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private DifficultySettings diffSettings;
    [SerializeField] private TimerScript timer;
    [SerializeField] private Slider messMeter;
    //[SerializeField] private string 

    private void Start()
    {
        if (!diffSettings)
            diffSettings = FindObjectOfType<DifficultySettings>();
        if (!menuHandler)
            menuHandler = FindObjectOfType<MenuHandler>();
        if (!_fpc)
            _fpc = FindObjectOfType<FirstPersonController>();
        if (!timer)
            timer = FindObjectOfType<TimerScript>();
        this.gameObject.SetActive(false);
    }

    public void SetLevelFinishedScreen(bool victory, string endLevelMessage)
    {
        gameObject.SetActive(true);
        if (!victory)
        {
            StartCoroutine(LevelCompleted(false, endLevelMessage, "Level failed!"));
            return;
        }
        StartCoroutine(LevelCompleted(true, endLevelMessage, "Level completed!"));

    }

    private IEnumerator LevelCompleted(bool victory, string message, string messageFinished)
    {
        BeforeWaitTime(victory, message);
        yield return new WaitForSeconds(waitTime);
        AfterWaitTime(victory, messageFinished);
    }

    private void BeforeWaitTime(bool nextLevelB, string message)
    {
        nextLevelButton.gameObject.SetActive(nextLevelB);
        buttons.SetActive(false);
        timeParent.SetActive(false);
        messParent.SetActive(false);
        messageText.text = message;// "You touched the floor!";
    }

    private void AfterWaitTime(bool victory, string message)
    {
        DisableGameOverlay();
        buttons.SetActive(true);
        messageText.text = message;
        if (victory)
        {
            timeParent.SetActive(true);
            timeText.text = timer.GetResult();
            messParent.SetActive(true);
            if (messMeter != null)
                GetMess();
            else
                NewRecord(timer.GetTimeInSeconds(), 0f);
        }            
        _fpc.RotateView(false);
        UnlockMouse();
#if UNITY_STANDALONE
        ProgressTracker.LevelCompleted(diffSettings.GetCurrentLevel() - 1);
#endif
#if UNITY_WEBGL
        ProgressTracker.finished[diffSettings.GetCurrentLevel() - 1] = 1;
#endif
        ProgressTracker.SaveData();
        Time.timeScale = 0;
    }

    private void GetMess()
    {
        var temp = (messMeter.value * 100).ToString();
        var posComma = temp.LastIndexOf(",");
        var posDot = temp.LastIndexOf(".");
        var pos = posComma == -1 ? posDot : posComma;
        var finalPos = pos == -1 ? 1 : pos;
        messText.text = temp.Substring(0, finalPos);
        NewRecord(timer.GetTimeInSeconds(), messMeter.value * 100);
    }

    private void UnlockMouse()
    {
        _fpc.m_MouseLook.SetCursorLock(false);
    }

    private void DisableGameOverlay()
    {
        menuHandler.GetGameOverlay().SetActive(false);
    }

    private void NewRecord(float timeInSeconds, float messMeter)
    {
        //link naar results voor level (progresstracker)
        //indien betere score record gameobject aanzetten
        //print("time in seconds: " + timeInSeconds);
        //print("mess meter " + messMeter);
#if UNITY_STANDALONE
        timeRecord.gameObject.SetActive(ProgressTracker.CheckForRecord(diffSettings.GetCurrentLevel() - 1, true, timeInSeconds));
        messRecord.gameObject.SetActive(ProgressTracker.CheckForRecord(diffSettings.GetCurrentLevel() - 1, false, messMeter));
#endif
#if UNITY_WEBGL
        ProgressTracker.times[diffSettings.GetCurrentLevel() - 1] = timeInSeconds;
        ProgressTracker.messes[diffSettings.GetCurrentLevel() - 1] = messMeter;
#endif
    }
}
