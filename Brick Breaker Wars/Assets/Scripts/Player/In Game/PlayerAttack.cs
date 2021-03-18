using UnityEngine;
using Mirror;
using TMPro;
public class PlayerAttack : NetworkBehaviour
{
    public static PlayerAttack instance = null;
    [SerializeField] private TMP_Text[] _targetTexts = null;
    [Command]
    public void CmdUsePowerUp(string user, string target, string matchID, PowerUps effect)
    {
        if (target == null) return;

        if (MatchMaker.instance.gameHandler.gameList.ContainsKey(matchID))
        {
            if (!MatchMaker.instance.gameHandler.gameList[matchID].playerInfo.ContainsKey(target))
            {
                Debug.LogError($"Target doesn't exist!");
                return;
            }
            if (user == null)
                UsePower(effect, target, matchID);
            else
            {
                Debug.Log($"Search for power up to use");
                UsePowerUp(user, target, matchID);
            }
        }
    }
    [Server]
    public void UsePowerUp(string user, string target, string matchID)
    {
        print(target);
        //check if target is alive
        
        if (MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.playerPowerUps.ContainsKey(user) &&
            MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.playerPowerUps[user].Count > 0)
        {
            Debug.Log($"Player {user} is attacking Player {target}");
            Debug.Log($"{MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.playerPowerUps[user]}");
            var powerUp = MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.playerPowerUps[user][0];
            MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.playerPowerUps[user].RemoveAt(0);
            if (powerUp == PowerUps.None) return;
            UsePower(powerUp, target, matchID);
            //UpdatePowerUpUI();
        }
    }
    [Server]
    private void UsePower(PowerUps toUse, string target, string matchID)
    {
        switch(toUse)
        {
            case PowerUps.AddLine:
                ServerMoveBlocks(target, matchID, 1);
                break;
            case PowerUps.RemoveLine:
                ServerMoveBlocks(target, matchID, -1);
                break;
            case PowerUps.None:
                break;
            default:
                break;
        }
    }
    [Server]
    private void ServerMoveBlocks(string target, string matchID, int direction)
    {
        if (MatchMaker.instance.gameHandler.gameList.ContainsKey(matchID))
        {
            MatchMaker.instance.gameHandler.gameList[matchID].playfieldSpawner.LowerLines(target, direction);
            MatchMaker.instance.gameHandler.gameList[matchID].playfieldSpawner.AddLine(target);
        }
    }
    private void Start()
    {
        if (hasAuthority && !Player.localPlayer.thisServer)
            instance = this;
    }
    private void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[0].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[1].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[2].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[3].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[4].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[5].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[6].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[7].text, Player.localPlayer.matchID, PowerUps.None);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                CmdUsePowerUp(Player.localPlayer.playerName, _targetTexts[8].text, Player.localPlayer.matchID, PowerUps.None);
        }
    }
}