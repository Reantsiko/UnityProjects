using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float damage = 0.1f;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 fireDirection = Vector2.down;
    
    public void FireAtPlayer() => fireDirection = (GameManager.instance.playerTransform.position - transform.localPosition).normalized;

    void Update()
    {
        transform.Translate(fireDirection * moveSpeed * Time.deltaTime);

        if (transform.position.y >= 20f || transform.position.y <= -7f)
            Destroy(gameObject);
    }
}
