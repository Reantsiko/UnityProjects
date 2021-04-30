using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Parser : MonoBehaviour
{
    public TwitchClient client;
    public Commands commands;
    private List<string> commandList;
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
            case PlayerCommands.commands:
                PrintCommands(userName);
                break;
            case PlayerCommands.create:
                commands?.CreatePlayer(userName, displayName, isPrivateMessage);
                break;
            case PlayerCommands.kill:
                commands?.KillPlayer(userName);
                break;
            /*case PlayerCommands.move:
                MovePlayer(userName, displayName, splitMessage);
                break;*/
            default:
                client.BotSendMessage(userName, $"@{displayName}, this command does not exist!", isPrivateMessage);
                break;
        }
    }

    private void MovePlayer(string userName, string displayName, string[] splitMessage)
    {
        if (GamePlayers.instance.createdPlayers.ContainsKey(userName) == true)
        {
            if (splitMessage.Length == 3 || splitMessage.Length == 5)
                Debug.Log($"Correct amount of inputs\ndoes this print on next line?\nthis as well?\nor are they all on the same line?");
        }
        else
            client.BotSendMessage(userName, $"{userName} you don't have a character to move!", false);
    }

    private void PrintCommands(string userName)
    {
        client.BotSendMessage(userName, $"The current commands are:", false);
        commandList.ForEach(c => client.BotSendMessage(userName, $"{c}", false));        
    }
}
