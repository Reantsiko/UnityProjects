using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Parser : MonoBehaviour
{
    public TwitchClient client;
    public Commands commands;
    [SerializeField] private List<string> commandList;
    private void Start()
    {
        if (client == null)
            client = FindObjectOfType<TwitchClient>();
        if (commands == null)
            commands = FindObjectOfType<Commands>();
        
        var temp = System.Enum.GetNames(typeof(PlayerCommands)).ToList();
        commandList = new List<string>();
        temp.ForEach(c => commandList.Add($"!{c}"));
    }
    public void ParseMessage(TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        var splitMessage = e.ChatMessage.Message.Split(' ');
        StartParsing(e.ChatMessage.Username, e.ChatMessage.DisplayName, splitMessage);
    }

    public void ParseMessage(TwitchLib.Client.Events.OnWhisperReceivedArgs e)
    {
        var splitMessage = e.WhisperMessage.Message.Split(' ');
        StartParsing(e.WhisperMessage.Username, e.WhisperMessage.DisplayName, splitMessage, true);
    }

    private void StartParsing(string userName, string displayName, string[] splitMessage, bool isPrivateMessage = false)
    {
        if (splitMessage.Length > 0 && !string.IsNullOrEmpty(splitMessage[0]) && splitMessage[0][0] == '!' && commandList != null && commandList.Count > 0)
        {
            if (commandList.Contains(splitMessage[0]))
            {
                var cmd = (PlayerCommands)commandList.IndexOf(splitMessage[0]);
                GetCorrectCommand(cmd, userName, displayName, splitMessage, isPrivateMessage);
            }
        }
    }

    private void GetCorrectCommand(PlayerCommands cmd, string userName, string displayName, string[] splitMessage, bool isPrivateMessage)
    {
        switch (cmd)
        {
            case PlayerCommands.create:
                CreatePlayer(userName, displayName, isPrivateMessage);
                break;
            case PlayerCommands.kill:
                KillPlayer(userName);
                break;
        }
    }

    private void CreatePlayer(string userName, string displayName, bool isPrivateMessage)
    {
        if (!GamePlayers.instance.createdPlayers.ContainsKey(userName))
        {
            if (commands?.CreatePlayer(userName, displayName) == true)
                client.BotSendMessage(userName, $"{displayName} has joined the game!", isPrivateMessage);
            else
                client.BotSendMessage(userName, $"{displayName}, there was an error on our side, could not create your character!", isPrivateMessage);
        }
        else
            client.BotSendMessage(userName, $"{displayName}, you already created a character!", isPrivateMessage);
    }

    private void KillPlayer(string userName)
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
