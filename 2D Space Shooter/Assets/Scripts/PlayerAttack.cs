﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;
    public Transform firePosition = null;
    public GameObject laserPrefab = null;
    [SerializeField] private string laserRef = null;

    private void Awake() => instance = this;

    public void Fire(Transform target)
    {
        if (firePosition == null) return;

        var laser = Instantiate(laserPrefab, firePosition.position, Quaternion.identity);
        var laserComponent = laser.GetComponent<PlayerLaser>();
        SoundPool.instance.Spawn(transform.position, laserRef);
        laserComponent.target = target;
    }
}
