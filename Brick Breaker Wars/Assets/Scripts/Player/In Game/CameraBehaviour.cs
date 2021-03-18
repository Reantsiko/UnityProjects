using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkTransformChild))]
[RequireComponent(typeof(NetworkTransformChild))]
[RequireComponent(typeof(NetworkSceneChecker))]
[RequireComponent(typeof(PaddleMovement))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(Ball))]
[RequireComponent(typeof(PlayerPowerUps))]
[RequireComponent(typeof(InGameMenu))]

public class CameraBehaviour : NetworkBehaviour
{
    /*
     * Variables
    */
    [Header("Cameras")]
    [SerializeField] private Camera _cam = null;
    [SerializeField] private Camera _enemyCam = null;
    [Header("UI Related")]
    [SerializeField] private Canvas _playerCanvas = null;
    [SerializeField] private GameObject[] _enemyCams = null;
    [SerializeField] private RenderTexture[] _enemyCamTextures = null;
    [SerializeField] private TMP_Text[] _playerNames = null;
    [SerializeField] private Transform _playfieldPosition = null;

    /*
     * Public Methods
    */

    /*
     * Private Methods
    */
    private void Start()
    {
        if (!Player.localPlayer.thisServer)
        {
            _cam.gameObject.SetActive(hasAuthority);
            _playerCanvas.gameObject.SetActive(hasAuthority);
            _enemyCam.gameObject.SetActive(!hasAuthority);
            ActivateEnemyViews();
            if (hasAuthority)
            {
                CmdSetEnemyCamTexture(Player.localPlayer.playerIndex);
                CmdCreatePlayField(Player.localPlayer.matchID, Player.localPlayer.playerName, _playfieldPosition.position);
            }
        }
    }
    [Command]
    private void CmdCreatePlayField(string matchID, string name, Vector3 pos)
    {
        StartCoroutine(MatchMaker.instance.gameHandler.gameList[matchID].playfieldSpawner.AddPlayField(name, pos));
        Debug.Log($"Set player block spawn ready");
    }
    [Command]
    private void CmdSetEnemyCamTexture(int playerIndex)
    {
        RpcSetEnemyCamTexture(playerIndex);
    }
    [ClientRpc]
    private void RpcSetEnemyCamTexture(int playerIndex)
    {
        if (playerIndex == Player.localPlayer.playerIndex) return;
        if (playerIndex < Player.localPlayer.playerIndex)
            _enemyCam.targetTexture = _enemyCamTextures[playerIndex - 1];
        else
            _enemyCam.targetTexture = _enemyCamTextures[playerIndex - 2];
    }

    private void ActivateEnemyViews()
    {
        var amount = ServerInstance.instance.playerInfo.Count - 1;
        var names = ServerInstance.instance.playerInfo.Keys.ToArray();
        for (int i = 0; i < amount; i++)
        {
            _enemyCams[i].SetActive(true);
            if (i < Player.localPlayer.playerIndex - 1)
                _playerNames[i].text = names[i];
            else
                _playerNames[i].text = names[i + 1];
        }
    }
}
