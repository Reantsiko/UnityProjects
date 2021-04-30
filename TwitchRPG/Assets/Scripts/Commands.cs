using UnityEngine;
using System.Collections;

public class Commands : MonoBehaviour
{
    public TwitchClient client;
    [SerializeField] private GameObject capsulePrefab = null;

    private void Start() => client = FindObjectOfType<TwitchClient>();
    public void CreatePlayer(string userName, string displayName, bool isPrivateMessage)
    {
        if (!GamePlayers.instance.createdPlayers.ContainsKey(userName))
        {
            if (SpawnPlayer(userName, displayName) == true)
                client.BotSendMessage(userName, $"{displayName} has joined the game!", isPrivateMessage);
            else
                client.BotSendMessage(userName, $"{displayName}, there was an error on our side, could not create your character!", isPrivateMessage);
        }
        else
            client.BotSendMessage(userName, $"{displayName}, you already created a character!", isPrivateMessage);
    }
    private bool SpawnPlayer(string userName, string displayName)
    {
        if (capsulePrefab != null)
        {
            var instance = Instantiate(capsulePrefab, new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-7f, 23f)), Quaternion.identity) as GameObject;
            var p = instance.GetComponent<Player>();
            p.userName = userName;
            p.displayName = displayName;
            p.gameObject.name = displayName;
            p.UpdateNameText();
            p.lastCommandTime = Time.time;
            p.playerStats = new PlayerStats(1, 5, 2, 10, 2, 2);
            GamePlayers.instance.createdPlayers.Add(userName, instance);
            return true;
        }
        return false;
    }

    public void KillPlayer(string userName)
    {
        if (GamePlayers.instance.createdPlayers.ContainsKey(userName))
        {
            var temp = GamePlayers.instance.createdPlayers[userName];
            GamePlayers.instance.createdPlayers.Remove(userName);
            Destroy(temp);
            client.BotSendMessage(userName, $"{userName} I have destroyed your character!", false);
        }
        else
            client.BotSendMessage(userName, $"{userName} you don't have a character to remove!", false);
    }
}
