using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Player : MonoBehaviour
{
    public string playerName = null;
    public TMP_Text playerNameText = null;

    public void UpdateNameText()
    {
        if (playerNameText == null)
            playerNameText = GetComponentInChildren<TMP_Text>();
        playerNameText.text = playerName;
    }
}
