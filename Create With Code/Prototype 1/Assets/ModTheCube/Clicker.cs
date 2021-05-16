using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    void Update()
    {
        if (Time.timeScale <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(pos, out RaycastHit hit))
            {
                if (hit.collider != null)
                {
                    Destroy(hit.collider.gameObject);
                    CubeSpawner.instance.SpawnNewCube();
                }
            }
        }
    }
}
