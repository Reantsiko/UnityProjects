using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDisabler : MonoBehaviour
{
    bool initialized;
    Button button = null;
    int level = 0;

    private void OnEnable()
    {
        if (!initialized)
            CalcAll();
        if (level == 1) { return; }
        if (level >= LevelProgressSaver.instance.levels)
        {
            button.interactable = false;
            return;
        }
        if (LevelProgressSaver.instance.finished[level - 1] == 0)
            button.interactable = false;
        else
            button.interactable = true;
    }

    private void CalcAll()
    {
        initialized = true;
        button = GetComponent<Button>();
        var posSpace = button.name.LastIndexOf(' ') + 1;
        var number = button.name.Substring(posSpace);
        level = int.Parse(number);
        //print(level);
    }
}
