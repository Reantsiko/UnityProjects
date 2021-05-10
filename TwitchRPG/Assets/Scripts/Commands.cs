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
        if (!CheckPlayerAndMessageLength(userName, 0, 0, true))
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
        if (!CheckPlayerAndMessageLength(userName, 0, 0)) return;

        var temp = GamePlayers.instance.createdPlayers[userName];
        GamePlayers.instance.createdPlayers.Remove(userName);
        Destroy(temp);
        client.BotSendMessage(userName, $"{userName} I have destroyed your character!", false);
    }

    public void PrintPlayerInfo(string userName)
    {
        if (!CheckPlayerAndMessageLength(userName, 0, 0)) return;

        var selectedPlayer = GamePlayers.instance.createdPlayers[userName];
        client.BotSendMessage(userName, $"{selectedPlayer.displayName} your active class is {selectedPlayer.playerClass.activeClass} " +
            $"and is level {selectedPlayer.playerClass.playerClass[selectedPlayer.playerClass.activeClass].level}", false);
        client.BotSendMessage(userName, $"{selectedPlayer.displayName} your active class is {selectedPlayer.playerJob.job} " +
            $"and is level {selectedPlayer.playerJob?.jobLevel.level}", false);
    }

    public void SetJob(string userName, string[] splitMessage)
    {
        if (!CheckPlayerAndMessageLength(userName, splitMessage.Length, 2)) return;
        var player = GamePlayers.instance.createdPlayers[userName];
        if (GamePlayers.instance.createdPlayers[userName].playerJob.job != PJob.none)
        {
            client.BotSendMessage(userName, $"{player.displayName} you already have a job!", false);
            return;
        }

        var jobValue = parser.playerJobList.IndexOf(splitMessage[1]);
        if (jobValue == -1)
        {
            client.BotSendMessage(userName, $"{player.displayName} the inputted job does not exist!", false);
            return;
        }
        player.SetJob((PJob)jobValue);  
    }
    public void SetClass(string userName, string[] splitMessage)
    {
        if (!CheckPlayerAndMessageLength(userName, splitMessage.Length, 2)) return;

        var player = GamePlayers.instance.createdPlayers[userName];
        var classValue = parser.playerClassList.IndexOf(splitMessage[1]);
        if (classValue == -1)
        {
            client.BotSendMessage(userName, $"{player.displayName} the inputted class does not exist!", false);
            return;
        }
        player.playerClass.ChangePlayerClass ((PClass)classValue);
    }

    public void SetAction(string userName, string[] splitMessage)
    {
        if (!CheckPlayerAndMessageLength(userName, splitMessage.Length, 2)) return;

        var index = parser.playerActions.IndexOf(splitMessage[1]);
        if (index < 0) return;
        var player = GamePlayers.instance.createdPlayers[userName];
        if ((ActiveAction)index == ActiveAction.work && player.playerJob.job == PJob.none)
            client.BotSendMessage(userName, $"{userName}, you have no job, set a job if you want to use this action!", false);
        else
        {
            player.activeAction = (ActiveAction)index;
            StartAction(player);
        }
    }

    private void StartAction(Player player)
    {
        switch (player.activeAction)
        {
            case ActiveAction.idle:
                player.StartIdle();
                break;
            case ActiveAction.work:
                player.StartJob();
                break;
            case ActiveAction.fight:
                break;
        }
    }

    private bool CheckPlayerAndMessageLength(string userName, int messageAmount, int messageAmountNeeded, bool isNew = false)
    {
        var exists = GamePlayers.instance.createdPlayers.ContainsKey(userName);
        var messageAm = messageAmount >= messageAmountNeeded;
        if (!isNew && !exists)
            client.BotSendMessage(userName, $"{userName}, you don't have an active character!", false);
        if (!messageAm)
            client.BotSendMessage(userName, $"{userName}, I need more information for your command!", false);
        return exists && messageAm;
    }
}
