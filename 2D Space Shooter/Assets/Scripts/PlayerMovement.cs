using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance = null;
    [SerializeField] private Vector3 moveTarget = Vector2.zero;
    [SerializeField] private float maxMoveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;
    //[SerializeField] private MoveField moveField = null;

    public void SetMoveTarget(Vector3 toSet) => moveTarget = toSet;
    private void MoveShip()
    {
        if (transform.position == moveTarget) return;
        transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
    }

    private void Start()
    {
        instance = this;
        GameManager.instance.playerTransform = transform;
        moveTarget = transform.position;
        //moveField = FindObjectOfType<MoveField>();
    }

    private void Update()
    {
        MoveShip();
    }
}
