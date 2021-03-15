using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFloorTouch : MonoBehaviour
{
    [SerializeField] private int maxWaitTime = 3;
    [SerializeField] private bool gameOver = false;
    [SerializeField] private bool _lavaActive = false;
    [SerializeField] private float _rayDistance = 1f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private bool _showCast = true;
    [SerializeField] private MenuHandler _menuHandler;
    // Start is called before the first frame update
    void Start()
    {
        if (!_menuHandler)
            _menuHandler = FindObjectOfType<MenuHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        RayCastMethod();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N))
            _lavaActive = !_lavaActive;
#endif
    }

    private void RayCastMethod()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, _rayDistance, _groundLayer);
        if (!gameOver && _lavaActive && hit.collider)
        {
            gameOver = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _menuHandler.OpenEndLevelScreen(false, "You touched the floor!");
        }
            //print("touching" + hit.collider.gameObject.name);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_showCast)
            Debug.DrawRay(transform.position, Vector3.down * _rayDistance, Color.red);
    }
#endif
    public IEnumerator FloorWillBecomeLava()
    {
        _menuHandler.ActivateCountdown(true);
        for (int i = maxWaitTime; i > 0; i--)
        {
            _menuHandler.CountdownMessage(i);
            yield return new WaitForSeconds(1);
        }
        _menuHandler.ActivateCountdown(false);
        _lavaActive = true;

        //groundRenderer.GetComponent<Renderer>().material = GroundButLava;
    }

    public void SetLavaActive(bool toSet) { _lavaActive = toSet; }
    public void SetGameOver(bool toSet) { gameOver = toSet; }
    public bool GetGameOver() { return gameOver; }
}
