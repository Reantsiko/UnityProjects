using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessHandler : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private DifficultySettings _settings;
    [SerializeField] private InteractableObjectsList objectList;
    [SerializeField] private float value;
    [SerializeField] private Color[] fillColors;
    [SerializeField] private Color[] backgroundColors;
    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private CheckFloorTouch _checkFloorTouch;
    [SerializeField] private MenuHandler _menuHandler;
    private Coroutine warningRoutine;
    private void Start()
    {
        if (!_slider)
            _slider = GetComponent<Slider>();
        if (!_settings)
            _settings = FindObjectOfType<DifficultySettings>();
        _slider.maxValue = _settings.GetSettings(ProgressTracker.difficulty).maxMessValue;
        if (!objectList)
            objectList = FindObjectOfType<InteractableObjectsList>();
        if (!_checkFloorTouch)
            _checkFloorTouch = FindObjectOfType<CheckFloorTouch>();
        if (!_menuHandler)
            _menuHandler = FindObjectOfType<MenuHandler>();
        StartCoroutine(UpdateSlider());
    }
    private IEnumerator UpdateSlider()
    {
        while (!_checkFloorTouch.GetGameOver())
        {
            value = 0;
            foreach (var o in objectList.GetList())
            {
                value += o.GetComponent<InteractableObject>().GetDistanceFromStart();
                
            }
            CalcPercentage();
            _slider.value = value;
            yield return new WaitForEndOfFrame();
        }
    }

    private void CalcPercentage()
    {
        var percentage = _slider.value / _slider.maxValue * 100;
        if (percentage >= 100 && warningRoutine == null)
            warningRoutine = StartCoroutine(WarningRoutine());
        if (percentage <= 33)
            SetColor(0);
        else if (percentage > 33 && percentage <= 66)
            SetColor(1);
        else
            SetColor(2);
    }

    private void SetColor(int col)
    {
        fill.color = fillColors[col];
        background.color = backgroundColors[col];
    }

    private IEnumerator WarningRoutine()
    {
        var time = _settings.GetSettings(ProgressTracker.difficulty).messFixTime;
        warningText.gameObject.SetActive(true);
        while (!_checkFloorTouch.GetGameOver() && _slider.value / _slider.maxValue * 100 >= 100)
        {
            warningText.text = "You made a big mess! You got " + time + " second(s) to clean it up before your parents get mad!";
            yield return new WaitForSeconds(1f);
            time--;
            if (time < 0)
            {
                _checkFloorTouch.SetGameOver(true);
                _menuHandler.OpenEndLevelScreen(false, "You failed to clean up!");
                break;
            }
        }
        warningText.gameObject.SetActive(false);
        warningRoutine = null;
    }
}
