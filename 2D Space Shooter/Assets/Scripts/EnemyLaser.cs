using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float damage = 0.1f;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 fireDirection;
    // Start is called before the first frame update
    void Start()
    {
        fireDirection = (GameManager.instance.playerTransform.position - transform.localPosition).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(fireDirection * moveSpeed * Time.deltaTime);
    }
}
