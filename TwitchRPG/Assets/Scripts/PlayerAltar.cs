using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAltar : MonoBehaviour
{
    public Transform spawnPosition;

    private void Start()
    {
        FindObjectOfType<Commands>().spawnPosition = spawnPosition;
        FindObjectOfType<TwitchClient>().StartClient();
    }

}
