using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance = null;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 moveTarget = Vector2.zero;
    [SerializeField] private float maxMoveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;
    //[SerializeField] private MoveField moveField = null;

    public void SetMoveTarget(Vector3 toSet) => moveTarget = toSet;
    public Vector3 GetMoveTarget() => moveTarget;
    private void MoveShip()
    {
        if (transform.position == moveTarget) return;
        transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
    }

    private void Awake() => instance = this;
    private void Start()
    {
        GameManager.instance.playerTransform = transform;
        startPos = transform.position;
        moveTarget = startPos;
    }

    private void Update() => MoveShip();

}
