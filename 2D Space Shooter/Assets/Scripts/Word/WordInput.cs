﻿using UnityEngine;
using System.Linq;
public class WordInput : MonoBehaviour
{
    private WordManager wordManager;

    private void Start() => wordManager = GetComponent<WordManager>();

    void Update()
    {
        if (!GameManager.instance.GetRespawning() && Time.timeScale != 0)
            Input.inputString.ToList().ForEach(l => wordManager.TypeLetter(l));
    }
}
