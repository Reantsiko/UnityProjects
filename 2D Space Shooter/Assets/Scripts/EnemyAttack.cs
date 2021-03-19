using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Transform firePosition = null;
    [SerializeField] private GameObject laserPrefab = null;

    public void FireAtPlayer()
    {
        Instantiate(laserPrefab, firePosition.position, Quaternion.identity);
    }
}
