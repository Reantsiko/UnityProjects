using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PowerUpBank : NetworkBehaviour
{
    /*
    * Variables
    */
    public ServerInstance serverInstance = null;
    public Dictionary<string, List<PowerUps>> playerPowerUps = null;
    [SerializeField] private Dictionary<string, PlayerPowerUps> _playerPowerUps = null;
    [SerializeField] private List<PowerUp> _powerUpPool = null;
    [SerializeField] private List<GameObject> _powerUpList = null;
    [SerializeField] private int _maxPowerUps = 8;
    //private Scene scene;
    /*
    * Public Methods
    */
    [Server]
    public void AddPlayerPowerUpList(string name)
    {
        var temp = new List<PowerUps>();
        if (playerPowerUps == null)
            playerPowerUps = new Dictionary<string, List<PowerUps>>();
        playerPowerUps.Add(name, temp);
        Debug.Log($"Added player {name} to the power up dictionary");
    }
    [Server]
    public void AddPowerUpToPlayer(string name, PowerUps toAdd)
    {
        print("Player that picked up powerup: " + name);
        if (playerPowerUps.ContainsKey(name))
        {
            if (playerPowerUps[name].Count < _maxPowerUps)
                playerPowerUps[name].Add(toAdd);
            print("Player " + name + " has " + playerPowerUps[name].Count + " powerups");
        }
        UpdatePowerUpUI();
    }
    [Server]
    public PowerUps GetPowerUp(string user)
    {
        if (playerPowerUps.ContainsKey(user) && playerPowerUps[user].Count > 0)
        {
            var power = playerPowerUps[user][0];
            playerPowerUps[user].RemoveAt(0);
            return power;
        }
        return PowerUps.None;
    }
    [Server]
    public void AddPowerUpToPool(PowerUp toAdd)
    {
        if (_powerUpPool == null)
            _powerUpPool = new List<PowerUp>();
        _powerUpPool.Add(toAdd);
    }
    [Server]
    public void SpawnPowerUp(string name, Vector3 powerUpSpawnPos)
    {
        if (name == null) return;

        if (_powerUpPool.Count > 0)
            SpawnFromPool(name, powerUpSpawnPos);
        else
            SpawnNewPowerUp(name, powerUpSpawnPos);
    }
    [Server]
    private void SetPowerUpInfo(PowerUp pu, string name, Vector3 spawnPos)
    {
        pu.RpcSetPowerUpInfo(name, true);
        pu.transform.position = spawnPos;
        pu.gameObject.SetActive(true);
    }
    [Server]
    public void AddBackToPool(PowerUp toAdd)
    {
        toAdd.transform.position = new Vector3(-100f, 0f, 0f);
        toAdd.RpcSetPowerUpInfo(string.Empty, false);
        toAdd.gameObject.SetActive(false);
        _powerUpPool.Add(toAdd);
    }
    /*
     * Private Methods
    */
    private void Start()
    {
        playerPowerUps = new Dictionary<string, List<PowerUps>>();
        _powerUpPool = new List<PowerUp>();
        _playerPowerUps = new Dictionary<string, PlayerPowerUps>();
    }
    [Server]
    public void PrefillPowerUpPool()
    {
        if (_powerUpList == null) return;
        if (_powerUpPool == null)
            _powerUpPool = new List<PowerUp>();
        foreach (var obj in _powerUpList)
        {
            for (int i = 0; i < 10; i++)
            {
                var powerUpObj = Instantiate(obj, new Vector3(-100f, 0f, 0f), Quaternion.identity);
                powerUpObj.SetActive(false);
                _powerUpPool.Add(powerUpObj.GetComponent<PowerUp>());
                SceneManager.MoveGameObjectToScene(powerUpObj, serverInstance.scene);
                NetworkServer.Spawn(powerUpObj);
            }
        }
    }
    [Server]
    public void AddPlayerPowerUps(string name, PlayerPowerUps toAdd)
    {
        if (_playerPowerUps == null)
            _playerPowerUps = new Dictionary<string, PlayerPowerUps>();
        _playerPowerUps.Add(name, toAdd);
        UpdatePowerUpUI();
    }
    [Server]
    private void UpdatePowerUpUI()
    {
        foreach (var name in _playerPowerUps.Keys)
        {
            //_playerPowerUps[name].UpdateImages(playerPowerUps[name]);
        }
    }
    [Server]
    private void SpawnFromPool(string name, Vector3 powerUpSpawnPos)
    {
        var selectedPower = Random.Range(0, _powerUpPool.Count);
        var toSpawn = _powerUpPool[selectedPower];
        _powerUpPool.Remove(toSpawn);
        var id = toSpawn.GetComponent<NetworkIdentity>();
        id.RemoveClientAuthority();
        id.AssignClientAuthority(serverInstance.playerInfo[name].GetComponent<NetworkIdentity>().connectionToClient);
        SetPowerUpInfo(toSpawn, name, powerUpSpawnPos);
    }
    [Server]
    private void SpawnNewPowerUp(string name, Vector3 powerUpSpawnPos)
    {
        var obj = _powerUpList[Random.Range(0, _powerUpList.Count)];
        var powerUpObj = Instantiate(obj, powerUpSpawnPos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(powerUpObj, serverInstance.scene);
        NetworkServer.Spawn(powerUpObj);
    }


    /*
     * Setter and Getter Methods
    */
}
