using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

public class TwitchClient : MonoBehaviour
{
    public static TwitchClient instance;
    public Client client;
    public Parser parser;
    private string channel_name = "reantsikox";

    private void Awake()
    {
        instance = this;
        if (parser == null)
            parser = FindObjectOfType<Parser>();
    }

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

    private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e) => parser.ParseMessage(e);
    private void Client_OnWhisperReceived(object sender, TwitchLib.Client.Events.OnWhisperReceivedArgs e) => parser.ParseMessage(e);

    

    public void BotSendMessage(string userName, string message, bool isPrivateMessage)
    {
        if (isPrivateMessage && !string.IsNullOrEmpty(userName))
            client.SendWhisper(userName, message);
        if (!isPrivateMessage)
            client.SendMessage(client.JoinedChannels[0], message);
    }
}
