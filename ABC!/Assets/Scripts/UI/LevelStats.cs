using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelStats : MonoBehaviour
{
    [SerializeField] public int levelNumber;
    [SerializeField] public string levelName;
    [SerializeField] private TMP_Text selectedLevel;
    [SerializeField] private TMP_Text bestTime;
    [SerializeField] private TMP_Text bestMess;
    [SerializeField] private GameObject back;
    [SerializeField] private LevelHandler levelHandler;

    private void Start()
    {
        if (!levelHandler)
            levelHandler = FindObjectOfType<LevelHandler>();
    }

    public void UpdateStats()
    {
        back.SetActive(false);
        gameObject.SetActive(true);
        selectedLevel.text = levelName;
#if UNITY_STANDALONE
        bestTime.text = GetValues(true, ProgressTracker.GetSaveData().BestTimesDifficulty);
        bestMess.text = GetValues(false, ProgressTracker.GetSaveData().BestMessesDifficulty);
#endif
#if UNITY_WEBGL
        bestTime.text = CalcTimeAsString(ProgressTracker.times[levelNumber - 1]);
        bestMess.text = GetMess(levelNumber - 1);
#endif
    }

#if UNITY_WEBGL
    private string GetMess(int level)
    {
        var mess = ProgressTracker.messes[level];
        if (mess == 0f && ProgressTracker.finished[level] == 0) return "N / A";
        return mess.ToString();
    }
    private string CalcTimeAsString(float timeInSeconds)
    {
        if (timeInSeconds == 0f) return "N / A";
        var timeAsString = timeInSeconds.ToString();
        string time;
        //Checking if there is a point or comma depending on operating system
        //for numbers after point/comma
        var posComma = timeAsString.LastIndexOf(',');
        var posPoint = timeAsString.LastIndexOf('.');

        if (posComma != -1 && posPoint == -1)
            time = ConvertTime(timeAsString, posComma);
        else if (posComma == -1 && posPoint != -1)
            time = ConvertTime(timeAsString, posPoint);
        else
            time = ConvertTime(timeAsString, -1);
        return time;
    }

    private string ConvertTime(string timeAsString, int commaPos)
    {
        string _seconds;
        string _ms;
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
        return _seconds + ":" + _ms;
    }
#endif

    private string CalculateTime(float timeInSeconds)
    {
        string _seconds, _ms;
        var timeAsString = timeInSeconds.ToString();
        var commaPos = timeAsString.LastIndexOf(",");
        var afterComma = timeAsString.Substring(commaPos + 1).Length;
        _seconds = timeAsString.Substring(0, commaPos);
        if (afterComma >= 2)
             _ms = timeAsString.Substring(commaPos + 1, 2);
        else if (afterComma == 1)
             _ms = timeAsString.Substring(commaPos + 1, 1) + "0";
        else
             _ms = "00";
        return _seconds + ":" + _ms;
    }

    public void CancelLevelSelect()
    {
        gameObject.SetActive(false);
        back.SetActive(true);
    }

    public void StartLevel()
    {
        levelHandler.LoadLevel(levelNumber);
        Debug.Log("Starting level " + levelNumber);
    }

    public string GetValues(bool isTime, Dictionary<Difficulty, float[]> dic)
    {
        if (dic[ProgressTracker.difficulty][levelNumber - 1] != 0)
        {
            if (isTime)
            {
                print("hello1");
                return CalculateTime(dic[ProgressTracker.difficulty][levelNumber - 1]);
            }
            else
            {
                print("hello2");
                return dic[ProgressTracker.difficulty][levelNumber - 1].ToString();
            }
        }
        return "N / A";
    }

    public void SetDifficulty(TMP_Dropdown dropdown)
    {
        ProgressTracker.difficulty = (Difficulty)dropdown.value;
        UpdateStats();
    }
}
