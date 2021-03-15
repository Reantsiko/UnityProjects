using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWorldCanvas : MonoBehaviour
{
    [SerializeField] Transform playerPos;

    private void Start()
    {
        if (playerPos == null)
            playerPos = FindObjectOfType<CharacterController>().transform;
    }

    void Update()
    {
        transform.LookAt(playerPos);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
