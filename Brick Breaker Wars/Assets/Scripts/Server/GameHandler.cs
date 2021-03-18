using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameHandler : NetworkBehaviour
{
    [Header("Other Server Components")]
    [SerializeField] private MatchMaker _matchMaker = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject _serverInstancePrefab = null;

    public Dictionary<string, ServerInstance> gameList = new Dictionary<string, ServerInstance>();

    private void Start()
    {
        _matchMaker = GetComponent<MatchMaker>();
    }
    
    public void BeginGame(string matchID)
    {
        MatchMaker.instance.SetGameAsStarted(matchID);
        ServerLoadScene();
        var serverInstanceObj = Instantiate(_serverInstancePrefab);
        serverInstanceObj.name = matchID;
        //NetworkServer.Spawn(serverInstanceObj, connectionToServer);
        serverInstanceObj.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
        gameList.Add(matchID, serverInstanceObj.GetComponent<ServerInstance>());
        AddPlayersToGame(matchID);
        
    }
    [Server]
    public void AddPlayersToGame(string matchID)
    {
        Debug.Log($"Adding players to the new game!");
        if (_matchMaker.matchList.ContainsKey(matchID) && gameList.ContainsKey(matchID))
        {
            foreach (var p in _matchMaker.matchList[matchID].players)
            {
                gameList[matchID].playerInfo.Add(p.name, p);
                gameList[matchID].MovePlayers();
            }
        }
    }

    public void GiveUseableScene(Scene scene)
    {
        foreach (var g in gameList.Values)
        {
            if (!g.sceneSet)
            {
                g.sceneSet = true;
                g.scene = scene;
                StartCoroutine(g.LoadPlayers(g.AllPlayersAdded));
                break;
            }
        }
    }

    public void FindGame(Scene scene)
    {
        foreach (var g in gameList.Values)
        {
            var igl = g.GetComponent<ServerInstance>();
            if (igl.sceneSet == false)
            {
                igl.sceneSet = true;
                igl.scene = scene;
                break;
            }
        }
    }

    [Server]
    private void ServerLoadScene()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
