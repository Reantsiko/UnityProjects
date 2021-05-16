using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Transform firePosition = null;
    [SerializeField] private GameObject laserPrefab = null;
    [SerializeField] private float minAttackWaitTime = 1f;
    [SerializeField] private float maxAttackWaitTime = 5f;
    [SerializeField] private string soundReference = null;
    private Coroutine fireAtPlayer = null;
    private void Start()
    {
        if (CheckAttack(Random.Range(0.00f, 1f)))
            fireAtPlayer = StartCoroutine(AttackDelay(Random.Range(minAttackWaitTime, maxAttackWaitTime)));
    }

    public Coroutine GetFireAtPlayer() => fireAtPlayer;

    private IEnumerator AttackDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(laserPrefab, firePosition.position, Quaternion.identity);
            SoundPool.instance.Spawn(transform.position);
        }
    }

    private bool CheckAttack(float val)
    {
        switch (GameManager.instance.difficulty)
        {
            case Difficulty.VeryEasy:
                return val < 0.1f;
            case Difficulty.Easy:
                return val < 0.15f;
            case Difficulty.Normal:
                return val < 0.2f;
            case Difficulty.Hard:
                return val < 0.3f;
            case Difficulty.VeryHard:
                return val < 0.4f;
            case Difficulty.Impossible:
                return val >= 0.5f;
        }
        return false;
    }

    public void FireAtPlayer()
    {
        var laserObj = Instantiate(laserPrefab, firePosition.position, Quaternion.identity) as GameObject;
        var laser = laserObj.GetComponent<EnemyLaser>();
        if (laser != null)
        {
            laser.FireAtPlayer();
            SoundPool.instance.Spawn(transform.position, soundReference);
        }
    }
}
