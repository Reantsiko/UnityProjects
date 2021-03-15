using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;
public class TimerScript : MonoBehaviour
{
    private CheckFloorTouch _floorTouch;
    private TMP_Text timerText = null;
    private FirstPersonController _fpc = null;
    private GameObject parent;
    private string _seconds = null;
    private string _ms = null;
    private float timeInSeconds;
    void Start()
    {
        if (!_floorTouch)
            _floorTouch = FindObjectOfType<CheckFloorTouch>();
        if (!_fpc)
            _fpc = FindObjectOfType<FirstPersonController>();
        if (!timerText)
            timerText = GetComponent<TMP_Text>();
        StartCoroutine(UpdateTimer());
    }

    IEnumerator UpdateTimer()
    {
        while (!_floorTouch.GetGameOver())
        {
            if (_fpc._hasMoved)
            {
                timeInSeconds = Time.timeSinceLevelLoad - _fpc.moveTime;
                CalcTimeAsString();
                timerText.text = _seconds + ":" + _ms;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public string GetResult()
    {
        CalcTimeAsString();
        return _seconds + ":" + _ms; 
    }


    private void CalcTimeAsString()
    {
        var timeAsString = timeInSeconds.ToString();
        //Checking if there is a point or comma depending on operating system
        //for numbers after point/comma
        var posComma = timeAsString.LastIndexOf(',');
        var posPoint = timeAsString.LastIndexOf('.');

        if (posComma != -1 && posPoint == -1)
            ConvertTime(timeAsString, posComma);
        else if (posComma == -1 && posPoint != -1)
            ConvertTime(timeAsString, posPoint);
        else
            ConvertTime(timeAsString, -1);
    }
    public float GetTimeInSeconds() { return timeInSeconds; }

    public void ConvertTime(string timeAsString, int commaPos)
    {
        if (commaPos != -1)
        {
            _seconds = timeAsString.Substring(0, commaPos);
            var temp = timeAsString.Substring(commaPos + 1);
            if (temp.Length > 2)
                _ms = temp.Substring(0, 2);
            else
                _ms = temp.Substring(0, 1) + "0";
        }
        else
        {
            _seconds = timeAsString;
            _ms = "00";
        }
    }
}
