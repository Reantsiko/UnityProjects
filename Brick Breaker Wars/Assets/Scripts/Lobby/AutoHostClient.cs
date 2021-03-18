using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class AutoHostClient : MonoBehaviour
{
    /*
     * Variables
    */
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField _nameInput = null;
    [SerializeField] private Button _connectButton = null;
    [Header("Player Name")]
    [Tooltip("This value is used for for saving the name to PlayPrefs")]
    [SerializeField] private string _key = "PlayerName";
    [Tooltip("Minimum name length")]
    [SerializeField] private int _minNameLength = 3;
    [SerializeField] private int _maxNameLength = 10;

    private NetworkManager _networkManager;

    /*
     * Public Methods
    */
    public void JoinLocal()
    {
        if (!_networkManager) return;

        _networkManager.networkAddress = "localhost";
        _networkManager.StartClient();
    }
    public void ConnectToServer()
    {
        if (_networkManager == null || _connectButton == null)
        {
            Debug.Log($"_networkManager or _connectButton not initialized in AutoHostClient.cs");
        }
        PlayerData.playerName = _nameInput.text;
        //_networkManager.networkAddress = "";
        _networkManager.StartClient();
    }
    public void SaveName()
    {
        if (_nameInput == null || _connectButton == null)
        {
            Debug.LogError($"_nameInput or _connectButton not initialized on AutoHostClient.cs!");
            return;
        }
        var name = _nameInput.text;
        PlayerPrefs.SetString(_key, _nameInput.text);
        _connectButton.interactable = name.Length >= _minNameLength && name.Length <= _maxNameLength;
    }
    /*
     * Private Methods
    */
    private void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        PlayerData.playerName = "Server";
        if (!Application.isBatchMode) //means headless build
        {
            if (PlayerPrefs.HasKey(_key))
            {
                _nameInput.text = PlayerPrefs.GetString(_key);
                SaveName();
            }
            Debug.Log($"=== Client Build ===");
        }
        else
        {
            PlayerData.playerName = "Server";
            Debug.Log($"=== Server Build ===");
        }
    }

}
