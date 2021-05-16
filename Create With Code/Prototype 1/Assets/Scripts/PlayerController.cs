using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Keybinds
{
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
}

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float turnSpeed = 10f;
    public Keybinds keybinds;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var turn = Input.GetKey(keybinds.left) ? -1 : 0;
        turn = Input.GetKey(keybinds.right) ? 1 : turn;
        var forward = Input.GetKey(keybinds.forward) ? 1 : 0;
        forward = Input.GetKey(keybinds.backward) ? -1 : forward;
        /*if (Input.GetKey(keybinds.forward))

        else if (Input.GetKey(keybinds.backward))*/
        transform.Translate(Vector3.forward * forward * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);
    }
}
