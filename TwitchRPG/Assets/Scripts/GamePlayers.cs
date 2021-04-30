using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamePlayers : MonoBehaviour
{
    public static GamePlayers instance;
    public Dictionary<string, GameObject> createdPlayers;

    [SerializeField] private float waitTimeBetweenDisconnectCheck = 60f;
    [SerializeField] private float disconnectTimer = 300f;
    private void Awake()
    {
        instance = this;
        createdPlayers = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        StartCoroutine(CheckIfPlayersAreStillOnline());
    }

    private IEnumerator CheckIfPlayersAreStillOnline()
    {
        WaitForSeconds waitTimer = new WaitForSeconds(waitTimeBetweenDisconnectCheck);
        while (true)
        {
            yield return waitTimer;

            var temp = (from p in createdPlayers.Values
                        where p.GetComponent<Player>().isOnline == true
                        select p.GetComponent<Player>()).ToList();
            temp.ForEach(p => p.isOnline = p.lastCommandTime + disconnectTimer > Time.time ? true : false);
            temp.ForEach(p => p.UpdateNameText());
        }
    }
}