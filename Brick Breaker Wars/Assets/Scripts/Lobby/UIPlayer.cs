using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayer : MonoBehaviour
{    
    /*
     * Variables
    */
    [SerializeField] private TMP_Text _playerNameText = null;

    /*
     * Public Methods
    */
    public void SetPlayer(Player player)
    {
        if (_playerNameText != null)
            _playerNameText.text = player.playerName;
    }
}
