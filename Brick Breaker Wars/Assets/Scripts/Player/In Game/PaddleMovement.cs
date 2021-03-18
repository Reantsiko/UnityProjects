using Mirror;
using UnityEngine;

public class PaddleMovement : NetworkBehaviour
{    
    /*
     * Variables
    */
    [SerializeField] private Transform _paddle = null; //Might be removed after rework how player spawns in.
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 30f;
    [SerializeField] Ball ball;
    [SerializeField] private KeyCode _left = KeyCode.A, _right = KeyCode.D, _rotateLeft = KeyCode.Q, _rotateRight = KeyCode.E; //change this to my keybinding system
    [SerializeField] private float minX = -7.7f;
    [SerializeField] private float maxX = -1.3f;
    [SerializeField] private float _minRotation = 135f;
    [SerializeField] private float _maxRotation = 225f;

    //private NetworkManagerLobby _networkManager = null;

    /*
    * Public Methods
    */

    /*
     * Private Methods
    */
    //private void Start() => _networkManager = FindObjectOfType<NetworkManagerLobby>();
    private void Update()
    {
        if (!hasAuthority || !ServerInstance.instance.hasGameStarted || !ServerInstance.instance.alivePlayer[Player.localPlayer.playerName]) return;

        MovePaddle();
        RotatePaddle();
    }
    private void MovePaddle()
    {
        if (Input.GetKey(_left))
            Move(-1);
        else if (Input.GetKey(_right))
            Move(1);
    }
    private void Move(float direction)
    {
        Vector2 pos = _paddle.transform.position;
        pos.x += direction * _moveSpeed * Time.deltaTime;
        Vector2 newPos = new Vector2(Mathf.Clamp(pos.x, minX, maxX), _paddle.localPosition.y);
        _paddle.localPosition = newPos;
    }
    private void RotatePaddle()
    {
        if (Input.GetKey(_rotateLeft))
            Rotate(1);
        else if (Input.GetKey(_rotateRight))
            Rotate(-1);
    }
    private void Rotate(float direction)
    {
        _paddle.transform.Rotate(Vector3.forward * direction * _rotateSpeed * Time.deltaTime);
        Vector3 rotationVector = _paddle.transform.localRotation.eulerAngles;
        rotationVector.z = Mathf.Clamp(rotationVector.z, _minRotation, _maxRotation);
        _paddle.transform.localRotation = Quaternion.Euler(rotationVector);
    }
    /*
     * Setter and Getter Methods
    */
    public Transform GetPaddle() { return _paddle; }
}
