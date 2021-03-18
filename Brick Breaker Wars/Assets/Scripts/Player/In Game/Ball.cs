using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : NetworkBehaviour
{
    /*
     * Variables
    */
    public bool isLaunched = false;
    [SerializeField] private Transform _ball = null; // might remove this variable after changing the player spawn system when game starts
    [SerializeField] private PaddleMovement _paddle = null;
    [SerializeField] private float _xPush = 2f;
    [SerializeField] private float _yPush = 7f;
    [SerializeField] private float _randomFactor = 0.2f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _ballToPaddleYOffset = 1f;
    [SerializeField] private bool _isAlive = true;

    private Vector2 _paddleToBallVector;
    private Rigidbody2D _rb;
    //private NetworkManagerLobby _networkManager;
    //TODO
    //make sure the speed of the ball is consistent
    /*
    * Public Methods
    */
    /*
     * Resets the ball if the player fails at hitting it with the paddle.
     * Method is called by DeathZone.cs
    */
    public void ResetBall()
    {
        isLaunched = false;
        _rb.velocity = Vector2.zero;
        _ball.position = _paddle.GetPaddle().position;
        if (hasAuthority)
            AddLineToPlayer(Player.localPlayer.playerName, Player.localPlayer.matchID);
    }
    /*
     * Private Methods
    */
    private void Start()
    {
        _paddleToBallVector = _ball.position - _paddle.GetPaddle().position;
        _rb = _ball.GetComponent<Rigidbody2D>();
    }
    private void AddLineToPlayer(string target, string matchID)
    {
        PlayerAttack.instance.CmdUsePowerUp(null, target, matchID, PowerUps.AddLine);
    }

    private void Update()
    {
        if (!hasAuthority || !ServerInstance.instance.hasGameStarted || !ServerInstance.instance.alivePlayer[Player.localPlayer.playerName]) { return; }
        if (!isLaunched) //change lowerBlocks.GetIsAlive() to a different method on the server
        {
            LockBallToPaddle();
            LaunchBall();
        }
    }
    /*
     * Locks the ball to the paddle before launch
    */
    private void LockBallToPaddle()
    {
        Vector2 paddlePos = _paddle.GetPaddle().position;
        _ball.position = paddlePos + _paddleToBallVector;
    }

    /*
     * Change the input to a keybinding
    */
    private void LaunchBall()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _xPush = Random.Range(2f, 5f);
            _xPush = transform.position.x < 5f ? _xPush : -_xPush; //change the 5f to a variable
            _rb.velocity = new Vector2(_xPush, _yPush);
            isLaunched = true;
        }
    }
    /*
     * Setter and Getter Methods
    */
    public void SetPaddleMovement(PaddleMovement toSet) { _paddle = toSet; }
    public void SetIsAlive(bool toSet) { _isAlive = toSet; }

    /*void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLaunched) return;
        print(collision.gameObject.tag);
        if (collision.gameObject.tag == "Block")
        {
            print("Hit block");
            var block = collision.gameObject.GetComponent<Block>();
            block.CmdDamageBlock();
        }

        Vector2 velocityTweak = new Vector2(
                                Random.Range(0, randomFactor),
                                Random.Range(0, randomFactor));
        rigidBody.velocity += velocityTweak;
    }*/


}
