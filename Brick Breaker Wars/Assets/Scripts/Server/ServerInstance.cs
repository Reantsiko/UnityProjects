using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

[System.Serializable]
public class ServerInstance : NetworkBehaviour
{
    /*
     * Variables
    */
    public static ServerInstance instance = null;
    public SyncDictionary<string, GameObject> playerInfo = new SyncDictionary<string, GameObject>();
    public SyncDictionary<string, bool> alivePlayer = new SyncDictionary<string, bool>();
    [Header("Game Related")]
    [SerializeField] private int blockPreSpawnAmount = 20;
    [SerializeField] private GameObject playFieldPrefab = null;
    [SyncVar]
    public bool hasGameStarted;
    [Header("Server Related")]
    [SerializeField] public PlayfieldSpawner playfieldSpawner = null;
    [SerializeField] public PowerUpBank powerUpBank = null;
    [SerializeField] private float _timeOutTimer = 120f;
    [Header("Scene related variables")]
    [SerializeField] private float _sceneUnloadTimer = 120f;
    [SerializeField] public bool sceneSet;
    [SerializeField] public Scene scene;
    
    /*
     * Public Methods
    */
    /*
     * Private Methods
    */
    private void Awake()
    {
        if (!Player.localPlayer.thisServer)
            instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void MovePlayers()
    {
        foreach (var player in playerInfo.Values)
        {
            var pInfo = player.GetComponent<Player>();
            player.transform.position = new Vector3(0f, 50f * pInfo.playerIndex, 0f);
        }
    }
    [Server]
    public IEnumerator LoadPlayers(Func<bool> CheckIfPlayersAdded)
    {
        yield return new WaitUntil(CheckIfPlayersAdded);
        Debug.Log($"All players are added!");
        SceneManager.MoveGameObjectToScene(gameObject, scene);
        NetworkServer.Spawn(gameObject, connectionToServer);
        LoadPlayers(gameObject.name);
        StartCoroutine(InitPlayers(AllPlayersLoaded));
    }
    [Server]
    private void LoadPlayers(string matchID)
    {
        if (MatchMaker.instance.matchList.ContainsKey(matchID) && MatchMaker.instance.gameHandler.gameList.ContainsKey(matchID))
        {
            foreach (var p in MatchMaker.instance.matchList[matchID].players)
            {
                p.GetComponent<Player>().StartGame();
            }
        }
    }
    [Server]
    public bool AllPlayersAdded() => playerInfo.Count == MatchMaker.instance.matchList[gameObject.name].players.Count;
    [Server]
    public bool AllPlayersLoaded()
    {
        foreach(var player in playerInfo.Values)
        {
            if (!player.GetComponent<Player>().playerLoaded)
                return false;
        }
        return true;
    }
    [Server]
    public void CheckForActivePlayers()
    {
        if (playerInfo.Count == 0)
            StartCoroutine(SceneUnloadCountdown());
    }
    [Server]
    private IEnumerator SceneUnloadCountdown()
    {
        Debug.Log($"Scene {scene.handle} used by gameID {gameObject.name} will be unloaded in 2 minutes!");
        yield return new WaitForSeconds(_sceneUnloadTimer);
        UnloadGameInstance();
    }
    [Server]
    private void UnloadGameInstance()
    {
        Debug.Log($"Unloading Scene {scene.handle} used by gameID {gameObject.name}!");
        SceneManager.UnloadSceneAsync(scene);
        Destroy(gameObject);
    }
    private IEnumerator PlayerTimeOut()
    {
        yield return new WaitForSeconds(_timeOutTimer);
    }
    [Server]
    private IEnumerator InitPlayers(Func<bool> AllPlayersLoaded)
    {
        yield return new WaitUntil(AllPlayersLoaded);
        foreach (var player in playerInfo.Values)
        {
            Debug.Log($"{player.name}");
            alivePlayer.Add(player.name, true);
            var playField = Instantiate(playFieldPrefab, player.transform.position, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(playField, scene);
            NetworkServer.Spawn(playField, player.GetComponent<NetworkIdentity>().connectionToClient);
        }
        //waituntil all players ready
        StartCoroutine(PreSpawnBlocks());
    }
    [Server]
    private IEnumerator PreSpawnBlocks()
    {
        var blocksToSpawn = blockPreSpawnAmount * playerInfo.Count;
        for (int i = 0; i < blocksToSpawn; i++)
        {
            playfieldSpawner.PreSpawnNewBlock();
            yield return new WaitForEndOfFrame();
        }
        //ping players for prespawn
        //answer all ready
        StartCoroutine(PreSpawnPowerUps());
    }
    [Server]
    private IEnumerator PreSpawnPowerUps()
    {
        foreach(var playerName in playerInfo.Keys)
        {
            powerUpBank.AddPlayerPowerUpList(playerName);
            yield return new WaitForEndOfFrame();
        }
        powerUpBank.PrefillPowerUpPool();
        yield return new WaitForSeconds(5f);
        //spawn logic
        //ping players for prespawn
        //answer all ready
        StartCoroutine(StartGameCountDown());
    }
    [Server]
    private IEnumerator StartGameCountDown()
    {
        for (int i = 5; i > 0; i--)
        {
            Debug.Log($"Game will begin in {i} seconds.");
            yield return new WaitForSeconds(1f);
        }
        hasGameStarted = true;
    }
}
