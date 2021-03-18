using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkSceneChecker))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PowerUp : NetworkBehaviour
{
    /*
     * Variables
    */
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private string _owner = null;
    [SerializeField] private PowerUps _powerUp = PowerUps.None;

    /*
    * Public Methods
    */
    public void CollisionDetected(bool isPickedUp, string name, string matchID, PowerUps powerUp)
    {
        if (hasAuthority)
            CmdDestroyPowerUp(isPickedUp, name, matchID, powerUp);
    }


    /*
     * Private Methods
    */
    private void Start()
    {
        if (_powerUp == PowerUps.None)
            Debug.LogError("_powerUp has no power up selected!");
    }
    private void Update()
    {
        //if (!hasAuthority) return;
        if (isServer)
            transform.Translate(Vector2.down * _moveSpeed * Time.deltaTime);
    }
    [Command]
    private void CmdDestroyPowerUp(bool isPickedUp, string name, string matchID, PowerUps powerUp)
    {
        if (isPickedUp)
            MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.AddPowerUpToPlayer(name, powerUp);
        MatchMaker.instance.gameHandler.gameList[matchID].powerUpBank.AddBackToPool(this);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Paddle")
        {
            CollisionDetected(true, Player.localPlayer.playerName, Player.localPlayer.matchID, _powerUp);
        }
    }
    /*
    * Setter and Getter Methods
    */
    public PowerUps GetPower() => _powerUp;
    public void RpcSetOwnerPU(string toSet)
    {
        print("Setting owner to: " + toSet);
        _owner = toSet;
    }
    [ClientRpc]
    public void RpcSetPowerUpInfo(string owner, bool value)
    {
        _owner = owner;
        gameObject.SetActive(value);
    }
}
