using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILobby : MonoBehaviour
{
    /*
     * Variables
    */
    public static UILobby instance = null;
    [Header("Join")]
    [SerializeField] private TMP_InputField _joinMatchInput = null;
    [SerializeField] private GameObject _gameFinderMenu = null;
    [SerializeField] private GameObject _lobbyMenu = null;
    [SerializeField] private GameObject _searchScreen = null;
    [Header("Lobby")]
    [SerializeField] private Transform _UIPlayerParent = null;
    [SerializeField] private TMP_Text _matchIDText = null;
    [SerializeField] private GameObject _startGameButton = null;

    [Header("Utility")]
    [SerializeField] private List<Selectable> _selectables = null;
    [Tooltip("Prefab to spawn upon entering a room.")]
    [SerializeField] private GameObject _UIPlayerPrefab = null;

    private bool _isSearching = false;
    private GameObject _playerLobbyUI;

    /*
     * Public Methods
    */
    public void HostGame(bool isPublic)
    {
        ChangeInteractable(false);

        Player.localPlayer.HostGame(isPublic);
    }

    public void HostSuccess(bool success, string matchID)
    {
        if (success && _lobbyMenu != null)
        {
            ChangeMenus(false, true, true);
            _playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            if (_matchIDText != null)
                _matchIDText.text = matchID;
        }
        else
            ChangeInteractable(!success);
    }
    public void Join()
    {
        ChangeInteractable(false);

        Player.localPlayer.JoinGame(_joinMatchInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            ChangeMenus(false, true);
            _playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            if (_matchIDText != null)
                _matchIDText.text = matchID;
        }
        else
            ChangeInteractable(!success);
    }
    public GameObject SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(_UIPlayerPrefab, _UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
        if (_matchIDText != null)
            _matchIDText.text = player.matchID;
        return newUIPlayer;
    }

    public void BeginGame()
    {
        Player.localPlayer.BeginGame();
    }

    public void SearchGame(bool val)
    {
        Debug.Log($"Searching for Game");
        _isSearching = val;
        _searchScreen.SetActive(val);
        StartCoroutine(SearchingForGame());
    }
    public void SearchSuccess(bool success, string matchID)
    {
        if (success)
        {
            SearchGame(false);
            JoinSuccess(success, matchID);
        }
    }

    public void DisconnectLobby()
    {
        if (_playerLobbyUI != null)
            Destroy(_playerLobbyUI);
        Player.localPlayer.DisconnectPlayer();
        ChangeMenus(true, false);
        ChangeInteractable(true);
    }
    /*
     * Private Methods
    */

    private void Start() => instance = this;
    private void ChangeInteractable(bool value) => _selectables.ForEach(x => x.interactable = value);
    private IEnumerator SearchingForGame()
    {
        WaitForSeconds checkEveryFewSeconds = new WaitForSeconds(1f);
        while (_isSearching)
        {
            yield return checkEveryFewSeconds;
            if (_isSearching)
                Player.localPlayer.SearchGame();
        }
    }
    private void ChangeMenus(bool finderMenu, bool lobbyMenu, bool isHost = false)
    {
        _startGameButton.SetActive(isHost);
        _gameFinderMenu.SetActive(finderMenu);
        _lobbyMenu.SetActive(lobbyMenu);
    }
}
