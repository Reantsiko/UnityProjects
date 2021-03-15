using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SettingsStruct
{
    [Tooltip("Value of the mess a player can make before the mess warning is activated.")]
    public float maxMessValue;
    [Tooltip("Time in seconds to give the player a chance of cleaning up the mess they made.")]
    public float messFixTime;
    [Tooltip("Value treshhold for when an object should start adding a value to the mess meter.")]
    public float messTreshhold;
}

public class DifficultySettings : MonoBehaviour
{
    [SerializeField] private int currentLevel = -1;
    [SerializeField] private bool _hasPowerUps = false;
    [SerializeField] private SettingsStruct easy;
    [SerializeField] private SettingsStruct normal;
    [SerializeField] private SettingsStruct hard;
    [SerializeField] private SettingsStruct impossible;

    private void Start()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }
    public SettingsStruct GetSettings(Difficulty difficulty)
    {
        switch(difficulty)
        {
            case Difficulty.easy:
                return easy;
            case Difficulty.normal:
                return normal;
            case Difficulty.hard:
                return hard;
            case Difficulty.impossible:
                return impossible;
        }
        return normal;
    }

    public int GetCurrentLevel() { return currentLevel; }
    public bool GetHasPowerups() { return _hasPowerUps; }
}


