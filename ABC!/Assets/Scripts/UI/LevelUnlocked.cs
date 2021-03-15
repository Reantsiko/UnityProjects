using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class LevelUnlocked : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private int levelNumber = -1;
    [SerializeField] private Button button;
    [SerializeField] private LevelStats levelStats;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!text)
            text = GetComponent<TMP_Text>();
        if (!button)
            button = GetComponent<Button>();
        if (!levelStats)
            levelStats = FindObjectOfType<LevelStats>();
        button.interactable = CheckIfUnlocked();
    }

    public void SelectLevel()
    {
        levelStats.levelName = text.text;
        levelStats.levelNumber = levelNumber;
        levelStats.UpdateStats();
    }
    private bool CheckIfUnlocked()
    {
        if (levelNumber == 1) return true;
#if UNITY_STANDALONE
        if (levelNumber != -1 &&
            levelNumber - 2 < ProgressTracker.GetSaveData().LevelProgress.Length &&
            ProgressTracker.GetSaveData().GetLevelCompleted(levelNumber - 2) == 1)
            return true;
#endif
#if UNITY_WEBGL
        if (levelNumber != -1 && levelNumber <= SceneManager.sceneCountInBuildSettings - 1 &&
            ProgressTracker.finished[levelNumber - 2] == 1)
            return true;
#endif
        return false;
    }

}
