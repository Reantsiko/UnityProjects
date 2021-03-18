using System;
using Mirror;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

[Serializable]
public class Match
{
    public string matchID;
    public string matchName;
    public int maxPlayers;
    public bool useTeams;
    public bool isPublic;
    public SyncListGameObject players = new SyncListGameObject();

    public Match(string matchID, GameObject player, bool isPublic, int maxPlayers)
    {
        this.matchID = matchID;
        this.isPublic = isPublic;
        this.maxPlayers = maxPlayers;
        this.matchName = $"{player.name}'s Game";
        players.Add(player);
    }
    public Match() { }
}

[Serializable]
public class SyncListGameObject : SyncList<GameObject> { }
[Serializable]
public class SyncListMatch : SyncList<Match> { }

public static class MatchExtensions
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}