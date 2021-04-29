﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    void Start()
    {    
        GetComponent<Renderer>().material.color = Random.ColorHSV();
    }
}
