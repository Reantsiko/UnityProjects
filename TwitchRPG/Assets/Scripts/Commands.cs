using UnityEngine;
using System.Linq;
using System.Collections;

public class Commands : MonoBehaviour
{
    public TwitchClient client;
    public Parser parser;
    public Transform spawnPosition;
    [SerializeField] private GameObject capsulePrefab = null;

    private void Start()
    {
        client = FindObjectOfType<TwitchClient>();
        parser = FindObjectOfType<Parser>();
    }
    public void CreatePlayer(string userName, string displayName, bool isPrivateMessage)
    {
        if (!CheckIfPlayerExists(userName, true))
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
            var instance = Instantiate(capsulePrefab, spawnPosition.position, Quaternion.identity) as GameObject;
            var p = instance.GetComponent<Player>();
            p.userName = userName;
            p.displayName = displayName;
            p.gameObject.name = displayName;
            p.InitializePlayer();
            p.UpdateNameText();
            p.lastCommandTime = Time.time;
            GamePlayers.instance.createdPlayers.Add(userName, p);
            return true;
        }
        return false;
    }

    public void KillPlayer(string userName)
    {
        if (!CheckIfPlayerExists(userName)) return;

        var temp = GamePlayers.instance.createdPlayers[userName];
        GamePlayers.instance.createdPlayers.Remove(userName);
        Destroy(temp);
        client.BotSendMessage(userName, $"{userName} I have destroyed your character!", false);
    }

    public void PrintPlayerInfo(string userName)
    {
        if (!CheckIfPlayerExists(userName)) return;

        var selectedPlayer = GamePlayers.instance.createdPlayers[userName];
        client.BotSendMessage(userName, $"{selectedPlayer.displayName} your active class is {selectedPlayer.playerClass.activeClass} " +
            $"and is level {selectedPlayer.playerClass.playerClass[selectedPlayer.playerClass.activeClass].level}", false);
        client.BotSendMessage(userName, $"{selectedPlayer.displayName} your active class is {selectedPlayer.playerJob.activeJob} " +
            $"and is level {selectedPlayer.playerJob.playerJob[selectedPlayer.playerJob.activeJob].level}", false);
    }

    public void SetJob(string userName, string[] splitMessage)
    {
        if (!CheckIfPlayerExists(userName)) return;

        if (splitMessage.Length > 1)
        {
            var player = GamePlayers.instance.createdPlayers[userName];
            var toFind = splitMessage[1];
            var jobValue = parser.playerJobList.IndexOf(toFind);
            if (jobValue == -1)
            {
                client.BotSendMessage(userName, $"{player.displayName} the inputted job does not exist!", false);
                return;
            }
            player.playerJob.ChangePlayerJob((PJob)jobValue);
        }       
    }
    public void SetClass(string userName, string[] splitMessage)
    {
        if (!CheckIfPlayerExists(userName)) return;

        if (splitMessage.Length > 1)
        {
            var player = GamePlayers.instance.createdPlayers[userName];
            var toFind = splitMessage[1];
            var classValue = parser.playerClassList.IndexOf(toFind);
            if (classValue == -1)
            {
                client.BotSendMessage(userName, $"{player.displayName} the inputted class does not exist!", false);
                return;
            }
            player.playerClass.ChangePlayerClass ((PClass)classValue);
        }
    }

    private bool CheckIfPlayerExists(string userName, bool isNew = false)
    {
        var exists = GamePlayers.instance.createdPlayers.ContainsKey(userName);
        if (!isNew && !exists)
            client.BotSendMessage(userName, $"{userName}, you don't have an active character!", false);
        return exists;
    }
}
