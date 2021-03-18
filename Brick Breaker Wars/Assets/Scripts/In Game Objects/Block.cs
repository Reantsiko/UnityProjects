using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkTransformChild))]
[RequireComponent(typeof(BoxCollider2D))]
[System.Serializable]
public class Block : NetworkBehaviour
{
    /*
     * Variables
    */
    [SerializeField] private Transform _childObject = null; // Might remove this depending on change in PlayField.cs
    [SyncVar]
    [SerializeField] public int _health = 3;
    [SyncVar]
    [SerializeField] public int _xPos = -1;
    [SyncVar]
    [SerializeField] public int _yPos = -1;
    [SyncVar]
    [SerializeField] public string _owner = null;
    [SyncVar]
    [SerializeField] public bool willDropPower;
    /*
    * Public Methods
    */
    [ClientRpc]
    public void RpcChangeActive(bool toSet)
    {
        gameObject.SetActive(toSet);
    }

    public void MoveChildObject(Vector3 toAdd) { _childObject.position += toAdd; } //Might remove this method depending on changes in PlayField.cs
    /*
     * Private Methods
    */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasAuthority)
        {
            if (collision.gameObject.tag == "Ball")
                CmdDestroyBlock(Player.localPlayer.matchID, Player.localPlayer.playerName, _childObject.position, willDropPower);
        }
    }

    /*
     * Command method to sync the health of the block hit over all clients.
     * In case a block is destroyed, it also asks to run the method AddBlockToPool(...) on the server.
     * Destroyed blocks get disabled and added in an object pool for re-use.
    */
    [Command]
    private void CmdDestroyBlock(string matchID, string playerName, Vector3 powerUpSpawnPos, bool willDrop)
    {
        --_health;
        if (_health < 1)
        {
            if (MatchMaker.instance.gameHandler.gameList.ContainsKey(matchID))
            {
                if (willDrop)
                    MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.SpawnPowerUp(playerName, powerUpSpawnPos);
                MatchMaker.instance.gameHandler.gameList[matchID].playfieldSpawner.AddBlockToPool(playerName, this);
            }
        }
    }
    /*
     * Setter and Getter Methods
    */
    public int GetXPos() { return _xPos; }
    public int GetYPos() { return _yPos; }
    public string GetOwner() { return _owner; }
}
