using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class Parser : MonoBehaviour
{
    public TwitchClient client;
    public Commands commands;
    [SerializeField] private List<string> commandList;
    [SerializeField] public List<string> playerClassList;
    [SerializeField] public List<string> playerJobList;
    [SerializeField] public List<string> playerActions;
    private void Start()
    {
        if (client == null)
            client = FindObjectOfType<TwitchClient>();
        if (commands == null)
            commands = FindObjectOfType<Commands>();

        commandList = CreateList<PlayerCommands>('!');//new List<string>();
        playerClassList = CreateList<PClass>();
        playerJobList = CreateList<PJob>();
        playerActions = CreateList<ActiveAction>();
    }

    private List<string> CreateList<TEnum>(char c = char.MinValue)
    {
        List<string> created = new List<string>();
        var listToUse = Enum.GetNames(typeof(TEnum)).ToList();
        listToUse.ForEach(x => created.Add($"{(c != char.MinValue ? c : ' ')}{x.ToLower()}".TrimStart(' ')));
        return created;
    }

    public void ParseMessage(TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        var fullMessage = e.ChatMessage.Message.ToLower();
        var splitMessage = fullMessage.Split(' ');
        StartParsing(e.ChatMessage.Username, e.ChatMessage.DisplayName, splitMessage);
    }

    public void ParseMessage(TwitchLib.Client.Events.OnWhisperReceivedArgs e)
    {
        var fullMessage = e.WhisperMessage.Message.ToLower();
        var splitMessage = fullMessage.Split(' ');
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
        Debug.Log($"Selected command: {cmd}");
        switch (cmd)
        {
            case PlayerCommands.commands:
                PrintCommands(userName);
                break;
            case PlayerCommands.create:
                commands?.CreatePlayer(userName, displayName, isPrivateMessage);
                break;
            case PlayerCommands.status:
                commands?.PrintPlayerInfo(userName);
                break;
            case PlayerCommands.setJob:
                commands?.SetJob(userName, splitMessage);
                break;
            case PlayerCommands.setClass:
                commands?.SetClass(userName, splitMessage);
                break;
            case PlayerCommands.setAction:
                commands?.SetAction(userName, splitMessage);
                break;
            /*case PlayerCommands.kill:
                commands?.KillPlayer(userName);
                break;*/
            /*case PlayerCommands.move:
                MovePlayer(userName, displayName, splitMessage);
                break;*/
            /*case PlayerCommands.repeat:
                StartRepeatObjective(userName, splitMessage);
                break;*/
            default:
                client.BotSendMessage(userName, $"@{displayName}, this command does not exist or is not yet implemented!", isPrivateMessage);
                break;
        }
    }

    private void StartRepeatObjective(string userName, string[] splitMessage)
    {
        if (splitMessage.Length >= 2)
        {
            var method = GetJobMethod(splitMessage[1]);
            if (method != null)
            {
                Debug.Log(method);
                var cor = StartCoroutine(method);
            }
        }
    }

    private string GetJobMethod(string msg)
    {
        if (string.Compare(msg, "job") == 0)
        {
            var startPos = MethodTester().ToString().IndexOf('<');
            var endPos = MethodTester().ToString().IndexOf('>');
            return $"{MethodTester().ToString().Substring(startPos + 1, endPos - startPos - 1)}";
        }
        return null;
    }

    private IEnumerator MethodTester()
    {
        Debug.Log($"Starting job!");
        yield return new WaitForSeconds(5f);
        Debug.Log($"Finished job!");
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
