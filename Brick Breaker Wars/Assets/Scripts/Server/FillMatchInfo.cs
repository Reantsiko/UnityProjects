using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FillMatchInfo : MonoBehaviour
{
    /*
     * Variables
    */
    public string matchID;
    [SerializeField] private TMP_Text _roomName = null;
    [SerializeField] private TMP_Text _players = null;
    [SerializeField] private TMP_Text _teamsText = null;

    /*
     * Public Methods
    */
    public void UpdateTexts()
    {
        if (_roomName == null || _players == null || _teamsText == null)
        {
            Debug.LogError($"_roomName, _players or _teamsText not initialized on Prefab!");
            return;
        }
        _roomName.text = $"{MatchMaker.instance.matchList[matchID].matchName}";
        _players.text = $"{MatchMaker.instance.matchList[matchID].players.Count}/{MatchMaker.instance.matchList[matchID].maxPlayers}";
        _teamsText.text = $"{(MatchMaker.instance.matchList[matchID].useTeams ? "Yes" : "No")}";
    }    public void JoinGame()
    {
        if (MatchMaker.instance.hasGameStarted.ContainsKey(matchID) && !MatchMaker.instance.hasGameStarted[matchID])
            Player.localPlayer.JoinGame(matchID);
        else
            gameObject.GetComponent<Button>().interactable = false;
    }
    /*
     * Private Methods
    */
}
