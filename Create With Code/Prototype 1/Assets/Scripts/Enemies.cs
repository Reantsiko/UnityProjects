using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float moveSpeed = 20f;
    
    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if (transform.position.y <= -10f)
            Destroy(gameObject);
    }
}
