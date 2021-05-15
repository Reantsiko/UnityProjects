using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float damage = 0.1f;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float screenXLimit = 10f;
    [SerializeField] private float screenYLimit = 5.5f;
    [SerializeField] private Vector2 fireDirection = Vector2.left;
    
    public void FireAtPlayer() => fireDirection = (GameManager.instance.playerTransform.position - transform.localPosition).normalized;

    void Update()
    {
        transform.Translate(fireDirection * moveSpeed * Time.deltaTime);

        if (transform.position.y >= screenYLimit || transform.position.y <= -screenYLimit ||
            transform.position.x >= screenXLimit || transform.position.x <= -screenXLimit)
            Destroy(gameObject);
    }
}
