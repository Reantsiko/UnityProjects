using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CleanUI : MonoBehaviour
{
    public TMP_Text cleanedText;

    private void Start()
    {
        cleanedText = GetComponentInChildren<TMP_Text>();
    }
}
