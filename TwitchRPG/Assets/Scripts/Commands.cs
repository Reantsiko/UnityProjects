using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands : MonoBehaviour
{
    [SerializeField] private GameObject capsulePrefab = null;

    public bool CreatePlayer(string userName, string displayName)
    {
        if (capsulePrefab != null)
        {
            var instance = Instantiate(capsulePrefab, new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-7f, 23f)), Quaternion.identity) as GameObject;
            var p = instance.GetComponent<Player>();
            p.userName = userName;
            p.displayName = displayName;
            p.gameObject.name = displayName;
            p.UpdateNameText();
            p.lastCommandTime = Time.time;
            GamePlayers.instance.createdPlayers.Add(userName, instance);
            return true;
        }
        return false;
    }
}
