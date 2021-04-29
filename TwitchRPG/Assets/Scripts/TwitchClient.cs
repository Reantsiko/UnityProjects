using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

public class TwitchClient : MonoBehaviour
{
    public Client client;
    private string channel_name = "reantsikox";
    [SerializeField] private GameObject capsulePrefab = null;
    public Dictionary<string, GameObject> createdPlayers = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        ConnectionCredentials credentials = new ConnectionCredentials("twitchsurvivalrpg", Secrets.bot_access_token);
        client = new Client();
        client.Initialize(credentials, channel_name);

        client.OnMessageReceived += Client_OnMessageReceived;
        client.OnWhisperReceived += Client_OnWhisperReceived;
        client.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            client.SendMessage(client.JoinedChannels[0], $"This is a test message from the bot!");
    }

    private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        Debug.Log($"The bot just read a message in chat");
        Debug.Log($"{sender.ToString()}");
        Debug.Log($"{e.ChatMessage.Username} said {e.ChatMessage.Message}");
        ParseMessage(e.ChatMessage.DisplayName, e.ChatMessage.Message);
    }

    private void Client_OnWhisperReceived(object sender, TwitchLib.Client.Events.OnWhisperReceivedArgs e)
    {
        ParseMessage(e.WhisperMessage.Username, e.WhisperMessage.Message);
    }

    private void ParseMessage(string playerName, string message)
    {
        var splitMessage = message.Split(' ');
        if (splitMessage.Length > 0 && !string.IsNullOrEmpty(splitMessage[0]) && splitMessage[0][0] == '!')
        {
            if (string.Compare(splitMessage[0], "!create") == 0)
            {
                if (!createdPlayers.ContainsKey(playerName))
                {
                    if (capsulePrefab != null)
                    {
                        var instance = Instantiate(capsulePrefab, new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-7f, 23f)), Quaternion.identity) as GameObject;
                        var p = instance.GetComponent<Player>();
                        p.playerName = playerName;
                        p.UpdateNameText();
                        createdPlayers.Add(playerName, instance);
                        client.SendMessage(client.JoinedChannels[0], $"{playerName} has joined the game!");
                    }
                }
                else
                    client.SendMessage(client.JoinedChannels[0], $"@{playerName} you already created a character!");
            }
            
        }
        else
            client.SendMessage(client.JoinedChannels[0], $"Does not compute!");
    }
}
