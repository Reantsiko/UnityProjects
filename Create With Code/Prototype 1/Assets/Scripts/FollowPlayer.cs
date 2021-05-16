using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform[] camPos;
    public KeyCode switchCamPos;
    int currPos = 0;
    void Start()
    {
        transform.position = camPos[currPos].position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(switchCamPos))
        {
            currPos++;
            if (currPos >= camPos.Length)
                currPos = 0;
            transform.position = camPos[currPos].position;
        }
    }
}
