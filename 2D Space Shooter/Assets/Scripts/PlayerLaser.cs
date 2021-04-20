using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    public Transform target = null;
    public float speed = 5f;
    [SerializeField] private string explosionRef = null;

    void Update()
    {
        if (target == null)
            Destroy(gameObject);
        transform.position = Vector3.MoveTowards(transform.position, target == null ? Vector3.zero : target.position, speed * Time.deltaTime);

        if (transform.position.y >= 20f || transform.position.y <= -7f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == target)
        {
            collision.GetComponent<WordDisplay>().RemoveWord();
            VFXPool.instance.Spawn(collision.transform.position);
            SoundPool.instance.Spawn(collision.transform.position, explosionRef);
            Destroy(gameObject);
        }
    }
}
