using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
[System.Serializable]
public class Player : NetworkBehaviour
{
    /*
     * Variables
    */
    public static Player localPlayer;
    [Header("Lobby Related")]
    [SyncVar]
    public string playerName;
    [SyncVar]
    public string matchID;
    [SyncVar]
    public int playerIndex;
    [Header("Game Related")]
    [SerializeField] private Scene scene;
    [SyncVar]
    public bool playerLoaded;
    [SyncVar]
    public int loadingProgress;
    [Header("General")]
    public bool thisServer;
    [SerializeField] private NetworkMatchChecker _matchChecker = null;
    GameObject playerUI;
    /*
     * Public Methods
    */
    public override void OnStartClient()
    {
        if (hasAuthority)
        {
            localPlayer = this;
            CmdSetName(PlayerData.playerName);
            if (isServer)
                thisServer = true;
        }
        else
        {
            Debug.Log($"Spawning player UI");
            playerUI = UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }
    public override void OnStopClient()
    {
        Debug.Log($"client stopped");
        Debug.Log($"Need to add more checks for correct DC");
        ClientDisconnect();
    }

    public override void OnStopServer()
    {
        Debug.Log($"client stopped on server");
        ServerDisconnect(this, this.matchID);
    }
    public void HostGame(bool isPublic)
    {
        var matchID = MatchMaker.instance.GetRandomMatchID();
        CmdHostGame(matchID, isPublic);
    }
    public void JoinGame(string matchID)
    {
        CmdJoinGame(matchID);
    }
    public void BeginGame()
    {
        CmdBeginGame();
    }
    public void StartGame()
    {
        TargetBeginGame();
    }
    public void SearchGame()
    {
        CmdSearchGame();
    }
    [Command]
    public void CmdSearchGame()
    {
        if (MatchMaker.instance.SearchGame(gameObject, out playerIndex, out matchID))
        {
            Debug.Log($"Found a game!");
            _matchChecker.matchId = matchID.ToGuid();
            TargetSearchGame(true, matchID, playerIndex);
        }
        else
        {
            Debug.Log($"No game found!");
            TargetSearchGame(false, matchID, playerIndex);
        }
    }
    [TargetRpc]
    public void TargetSearchGame(bool succes, string matchID, int playerIndex)
    {
        this.matchID = matchID;
        this.playerIndex = playerIndex;
        UILobby.instance.SearchSuccess(succes, matchID);
    }

    public void DisconnectPlayer()
    {
        CmdUpdateRoom(matchID, playerIndex);
        CmdDisconnectPlayer(this, this.matchID);
        CmdResetMatchInfo();
    }
    [Command]
    public void CmdUpdateRoom(string matchID, int indexOfLeftPlayer)
    {
        var playerAmount = MatchMaker.instance.matchList[matchID].players.Count;
        for (int i = indexOfLeftPlayer; i < playerAmount; i++)
        {
            MatchMaker.instance.matchList[matchID].players[i].GetComponent<Player>().playerIndex--;
        }
        MatchMaker.instance.matchList[matchID].players.RemoveAt(indexOfLeftPlayer - 1);
    }
    [Command]
    public void CmdDisconnectPlayer(Player player, string matchID)
    {
        ServerDisconnect(player, matchID);
    }
    [Command]
    public void CmdResetMatchInfo()
    {
        matchID = string.Empty;
        playerIndex = 0;
        /*isReady = false;
        isHost = false;*/
    }
    public void AddToInGameList(Scene scene)
    {
        Debug.Log($"Game scene handle: {scene.handle}");
        this.scene = scene;
    }
    [Command]
    public void CmdPlayerLoaded(bool val) => playerLoaded = val;
    /*
     * Private Methods
    */

    private void Awake()
    {
        _matchChecker = GetComponent<NetworkMatchChecker>();
        DontDestroyOnLoad(this);
    }
    [Command]
    private void CmdSetName(string name)
    {
        playerName = name;
        gameObject.name = playerName;
        TargetSetObjName(name);
    }
    [TargetRpc]
    private void TargetSetObjName(string name) => gameObject.name = name;


    [Command]
    private void CmdHostGame(string matchID, bool isPublic)
    {
        this.matchID = matchID;
        if (MatchMaker.instance.HostGame(matchID, gameObject, isPublic, out playerIndex))
        {
            Debug.Log($"Game hosted successfully.");
            _matchChecker.matchId = matchID.ToGuid();
            TargetHostGame(true, matchID, playerIndex);
        }
        else
        {
            Debug.LogError($"Game hosted failed.");
            TargetHostGame(false, matchID, playerIndex);
        }
    }

    [TargetRpc]
    private void TargetHostGame(bool success, string matchID, int playerIndex)
    {
        this.playerIndex = playerIndex;
        Debug.Log($"MatchID: {this.matchID} == {matchID}");
        UILobby.instance.HostSuccess(success, matchID);
    }


    [Command]
    private void CmdJoinGame(string matchID)
    {
        this.matchID = matchID;
        if (MatchMaker.instance.JoinGame(matchID, gameObject, out playerIndex))
        {
            Debug.Log($"Game joined successfully.");
            _matchChecker.matchId = matchID.ToGuid();
            TargetJoinGame(true, matchID, playerIndex);
        }
        else
        {
            Debug.LogError($"Failed to join game.");
            TargetJoinGame(false, matchID, playerIndex);
        }
    }

    [TargetRpc]
    private void TargetJoinGame(bool success, string matchID, int playerIndex)
    {
        this.playerIndex = playerIndex;
        Debug.Log($"MatchID: {this.matchID} == {matchID}");
        UILobby.instance.JoinSuccess(success, matchID);
    }


    [Command]
    private void CmdBeginGame()
    {
        MatchMaker.instance.gameHandler.BeginGame(matchID);
    }
    [TargetRpc]
    private void TargetBeginGame()
    { 
        SceneManager.LoadScene(2);
        Debug.Log($"MatchID: {matchID} beginning");
    }
    /*[Server]
    private void CmdServerLoad()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }*/

    [Server]
    private void ServerDisconnect(Player player, string matchID)
    {
        MatchMaker.instance.CmdPlayerDisconnected(player, matchID);
        _matchChecker.matchId = string.Empty.ToGuid();
        RpcDisconnectPlayer();
    }
    [ClientRpc]
    public void RpcDisconnectPlayer() => ClientDisconnect();
    private void ClientDisconnect()
    {
        if (playerUI != null)
            Destroy(playerUI);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            CmdRemovePlayer(matchID);
    }*/
}
