using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private KeyCode up = KeyCode.W;
    [SerializeField] private KeyCode down = KeyCode.S;
    [SerializeField] private KeyCode left = KeyCode.A;
    [SerializeField] private KeyCode right = KeyCode.D;
    [SerializeField] private float moveSpeed = 5f;

    
    private void Movement(float x, float y)
    {
        var moveVector = new Vector3(x, 0f, y);
        transform.Translate(moveVector * moveSpeed * Time.deltaTime);
        var temp = Mathf.Clamp(transform.position.y, 10f, 10f);
        transform.position = new Vector3(transform.position.x, temp, transform.position.z);
    }

    void Update()
    {
        float x = 0, y = 0;
        if (Input.GetKey(up))
            y = 1f;
        else if (Input.GetKey(down))
            y = -1f;
        if (Input.GetKey(right))
            x = 1f;
        else if (Input.GetKey(left))
            x = -1f;
        Movement(x, y);
    }
}
