using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class PlayerPowerUps : NetworkBehaviour
{
    [SerializeField] private Image[] _powerUpImages = null;
    [SerializeField] private Sprite[] _sprites = null;

    private void Start()
    {
        if (hasAuthority)
        {
            //AddToPowerBank(PlayerData.GetPlayerName());
        }
    }

    [ClientRpc]
    public void UpdateImages(List<PowerUps> powerUpList)
    {
        for (int i = 0; i < _powerUpImages.Length; i++)
        {
            print(powerUpList.Count);
            int powerUp;
            if (i >= powerUpList.Count)
                powerUp = 0;
            else
                powerUp = powerUpList[i].GetHashCode();
            _powerUpImages[i].sprite = _sprites[powerUp];
        }
    }

    [Command]
    private void AddToPowerBank(string name)
    {
        //PowerUpBank.instance.AddPlayerPowerUps(name, this);
    }
}
