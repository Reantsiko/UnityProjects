using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    [SerializeField] private Transform pos = null;
    [SerializeField] private LayerMask layer = 0;
    void Update()
    {
        RaycastHit hit;

        Physics.Raycast(transform.position, Vector3.down, out hit, layer);
        if (hit.collider)
        {
            pos.position = new Vector3(pos.position.x, hit.point.y + 0.01f, pos.position.z);
        }
    }
}
