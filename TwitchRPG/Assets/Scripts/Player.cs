﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Player : MonoBehaviour
{
    public string playerName = null;
    public TMP_Text playerNameText = null;
    public float lastCommandTime;
    public bool isOnline = true;
    public void UpdateNameText()
    {
        if (playerNameText == null)
            playerNameText = GetComponentInChildren<TMP_Text>();
        if (isOnline)
            playerNameText.text = $"<color=green>{playerName}</color>";
        else
            playerNameText.text = $"<color=red>{playerName}</color>";
    }
}
