using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class PlayfieldSpawner : NetworkBehaviour
{
    /*
     * Variables
    */
    [SerializeField] public ServerInstance serverInstance = null;
    [SerializeField] private int _currentBlockHealth = 1;
    [SerializeField] private int _linesToSpawn = 3;
    [SerializeField] private float _width = 7;
    [SerializeField] private float _height = 8;
    [SerializeField] private float _blockWidth = -1;
    [SerializeField] private float _blockHeight = -1;
    [Range(0, 1f)]
    [SerializeField] private float _dropChance = 0.15f;
    [SerializeField] private GameObject _blockPrefab = null;
    [SerializeField] private Vector3 _hiddenLocation = new Vector3(-100f, 0f, 0f);


    private Dictionary<string, PlayField> _playFields = null;
    [SerializeField] private List<Block> _blockPool;

    /*
    * Public Methods
    */
    [Server]
    public void RemoveAuthority(string name)
    {
        var temp = _playFields[name];
        foreach (var b in temp._blockList)
        {
            b.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        }
    }
    /*
     * Created playfield at the player's position.
    */
    [Server]
    public IEnumerator AddPlayField(string name, Vector3 playerPos)
    {
        Debug.Log($"Adding playfield for {name}");
        yield return new WaitForEndOfFrame();
        PlayField temp = new PlayField(_width, _height, _blockWidth, _blockHeight, playerPos, true);
        _playFields.Add(name, temp);
        StartCoroutine(SpawnStart(name, _linesToSpawn));
    }
    /*
     * Adds a line of blocks for player 'name'.
    */
    [Server]
    public void AddLine(string name)
    {
        for (int i = 2; i <= 11; i++)
            SpawnBlock(name, i, 0);
    }
    /*
     * Lowers or brings up the lines for player 'name' depending on the yDirection.
     * In case of bringing lines up, have to add code that in case the blocks are on row 0,
     * they get destroyed.
    */
    [Server]
    public void LowerLines(string name, int yDirection = 1)
    {
        int lastRow = _playFields[name]._fieldArray.GetLength(1) - 1;
        int rowLength = _playFields[name]._fieldArray.GetLength(0);
        for (int y = lastRow; y >= 0; y--)
        {
            for (int x = 0; x < rowLength; x++)
            {
                if (y == lastRow && _playFields[name]._fieldArray[x, y] != null && _playFields[name]._fieldArray[x, y].gameObject.activeSelf == true)
                    Debug.Log($"{name} has lost");
                    //serverInstance.alivePlayer[name] = false;
                ChangeBlockPosition(name, x, y, lastRow, yDirection);
            }
        }
    }
    /*
     * Adds a block to the _blockPool upon destruction.
    */
    public void AddBlockToPool(string name, Block toAdd)
    {
        if (toAdd == null) return;
        if (toAdd.GetXPos() == -1 || toAdd.GetYPos() == -1 || toAdd.GetOwner() == null)
        {
            Destroy(toAdd.gameObject);
            return;
        }

        if (_blockPool == null)
            _blockPool = new List<Block>();
        _playFields[name].SetValue(toAdd.GetXPos(), toAdd.GetYPos(), null);
        _playFields[name].RemoveBlockFromList(toAdd);
        SetBlockInfo(toAdd, null, -1, -1);
        toAdd.transform.position = _hiddenLocation;
        _blockPool.Add(toAdd);
    }
    /*
     * Private Methods
    */
    private void Awake()
    {
        _playFields = new Dictionary<string, PlayField>();
        _blockPool = new List<Block>();
    }
    /*
     * Creates starting field for the player 'name'.
    */
    [Server]
    private IEnumerator SpawnStart(string name, int lines)
    {
        Debug.Log($"Spawning start for {name}");
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < lines; i++)
        {
            AddLine(name);
            LowerLines(name);
        }
        AddLine(name);
    }
    /*
     * Creates (new) blocks for usage in the game.
     * TO ADD
     * object pool for the player for easier manipulation of blocks when powerups are used.
    */
    [Server]
    private void SpawnBlock(string name, int x, int y)
    {
        if (_blockPool.Count == 0)
            SpawnNewBlock(name, x, y);
        else
            SpawnBlockFromList(name, x, y);
    }
    /*
     * Instantiates a new block if the block pool is empty, assigns it to player 'name' and spawns it on all clients.
    */
    [Server]
    private void SpawnNewBlock(string name, int x, int y)
    {
        var blockObject = Instantiate(_blockPrefab, _playFields[name].GetWorldPosition2D(x, -y), Quaternion.identity);
        var block = blockObject.GetComponent<Block>();
        _playFields[name].SetValue(x, y, block);
        _playFields[name].AddBlockToList(block);
        block.MoveChildObject(new Vector3(_blockWidth / 2, -_blockHeight / 2));
        SceneManager.MoveGameObjectToScene(blockObject, serverInstance.scene);
        NetworkServer.Spawn(blockObject, serverInstance.playerInfo[name].GetComponent<NetworkIdentity>().connectionToClient);
        SetBlockInfo(block, name, x, y);
    }
    [Server]
    public void PreSpawnNewBlock()
    {
        var blockObject = Instantiate(_blockPrefab, _hiddenLocation, Quaternion.identity);
        var block = blockObject.GetComponent<Block>();
        _blockPool.Add(block);
        SceneManager.MoveGameObjectToScene(blockObject, serverInstance.scene);
        NetworkServer.Spawn(blockObject);
        SetBlockInfo(block, null, 0, 0);
        block.MoveChildObject(new Vector3(_blockWidth / 2, -_blockHeight / 2));
    }
    /*
     * Takes a block from the object pool and initializes it to be spawned in the field of player 'name'.
    */
    [Server]
    private void SpawnBlockFromList(string name, int x, int y)
    {
        if (_blockPool.Count == 0)
        {
            SpawnNewBlock(name, x, y);
            return;
        }
        var block = _blockPool[0];
        _blockPool.Remove(block);
        var id = block.GetComponent<NetworkIdentity>();
        id.RemoveClientAuthority();
        SetBlockInfo(block, name, x, y);
        id.AssignClientAuthority(serverInstance.playerInfo[name].GetComponent<NetworkIdentity>().connectionToClient);
        _playFields[name].SetValue(x, y, block);
        _playFields[name].AddBlockToList(block);
        block.transform.position = _playFields[name].GetWorldPosition2D(x, -y);
    }
    [Server]
    private void WillDropPower(Block b)
    {
        var chance = Random.value;
        if (chance <= _dropChance)
            b.willDropPower = true;
    }
    /*
     * Sets the information for the block such as it's owner and positions in the fieldArray. 
    */
    [Server]
    private void SetBlockInfo(Block block, string name, int x, int y)
    {
        block._health = _currentBlockHealth;
        block._xPos = x;
        block._yPos = y;
        block._owner = name;
        WillDropPower(block);
        //block.RpcSetTexture(); method not yet created
    }
    /*
     * Changes the position of the blocks if new position is valid.
    */
    [Server]
    private void ChangeBlockPosition(string name, int x, int y, int lastRow, int yDirection, int xDirection = 0)
    {
        if (y < lastRow && _playFields[name]._fieldArray[x, y] != null && _playFields[name]._fieldArray[x, y].gameObject.activeSelf == true)
        {
            var temp = _playFields[name]._fieldArray[x, y];
            _playFields[name]._fieldArray[x, y].transform.position = _playFields[name].GetWorldPosition2D(x, -y - 1);
            _playFields[name]._fieldArray[x, y] = null;
            _playFields[name]._fieldArray[x, y + yDirection] = temp;
            temp._xPos = x;
            temp._yPos = y + yDirection;
            //temp.ServerSetXYPos(x, y + yDirection);
        }
    }
    /*
     * Makes sure if the position is valid (NOT YET IN USE)
    */
    private bool PositionCheck(string name, int x, int y, int lastRow, int yDirection, int xDirection)
    {
        if (y < lastRow &&
            y + yDirection < lastRow &&
            _playFields[name]._fieldArray[x, y] != null &&
            _playFields[name]._fieldArray[x, y].gameObject.activeSelf == true)
            return true;
        return false;
    }
    /*
     * Setter and Getter Methods
    */
}
