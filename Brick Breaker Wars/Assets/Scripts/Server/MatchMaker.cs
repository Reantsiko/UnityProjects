using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class MatchMaker : NetworkBehaviour
{
    /*
     * Variables
    */
    public static MatchMaker instance = null;
    [Header("Other Server Components")]
    public GameHandler gameHandler = null;
    [Header("Match ID")]
    [SerializeField] private int _matchIDLength = 7;
    public SyncDictionary<string, Match> matchList = new SyncDictionary<string, Match>();
    public SyncDictionary<string, bool> hasGameStarted = new SyncDictionary<string, bool>();
    /*
     * Public Methods
    */
    public string GetRandomMatchID()
    {
        string id = string.Empty;

        for (int i = 0; i < _matchIDLength; i++)
        {
            int random = Random.Range(0, 36);
            if (random < 26)
                id += (char)(random + 65);
            else
                id += (random - 26).ToString();
        }
        Debug.Log($"Random match ID: {id}");
        return id;
    }
    public bool HostGame(string matchID, GameObject player, bool isPublic, out int playerIndex)
    {
        playerIndex = -1;
        if (!matchList.ContainsKey(matchID) && 
            !hasGameStarted.ContainsKey(matchID))
        {
            matchList.Add(matchID, new Match(matchID, player, isPublic, 10));
            hasGameStarted.Add(matchID, false);
            playerIndex = 1;
            Debug.Log("Match generated");
            return true;
        }
        Debug.LogError("Match ID exists already");
        return false;
    }
    [Server]
    public bool JoinGame(string matchID, GameObject player, out int playerIndex)
    {
        playerIndex = -1;
        if (matchList.ContainsKey(matchID) && 
            hasGameStarted.ContainsKey(matchID) && 
            !hasGameStarted[matchID] && 
            matchList[matchID].players.Count < matchList[matchID].maxPlayers)
        {
            matchList[matchID].players.Add(player);
            playerIndex = matchList[matchID].players.Count;
            Debug.Log("Match joined");
            return true;
        }
        Debug.LogError("Match ID does not exist");
        return false;
    }
    public bool SearchGame(GameObject player, out int playerIndex, out string matchID)
    {
        playerIndex = -1;
        matchID = string.Empty;

        foreach (var m in matchList.Values)
        {
            if (m.isPublic && m.players.Count < m.maxPlayers)
            {
                matchID = m.matchID;
                if (JoinGame(matchID, player, out playerIndex))
                    return true;
                else
                    matchID = string.Empty;
            }
        }
        return false;
    }
    [Server]
    public void CmdPlayerDisconnected(Player player, string matchID)
    {
        Debug.Log($"{player.playerName}, {matchID}");
        if (matchList.ContainsKey(matchID))
        {
            Debug.Log($"Player disconnected from match {matchID} | {matchList[matchID].players.Count} players remaining.");
            if (matchList[matchID].players.Count <= 0)
            {
                matchList.Remove(matchID);
                if (hasGameStarted.ContainsKey(matchID))
                    hasGameStarted.Remove(matchID);
            }
        }
    }
    [Server]
    public void SetGameAsStarted(string matchID)
    {
        Debug.Log($"Is game still in matchList? {matchList.ContainsKey(matchID)}");
        if (matchList.ContainsKey(matchID) && hasGameStarted.ContainsKey(matchID))
            hasGameStarted[matchID] = true;
    }
    /*
     * Private Methods
    */
    private void Awake()
    {
        instance = this;
        gameHandler = GetComponent<GameHandler>();
        DontDestroyOnLoad(this);
    }
}


