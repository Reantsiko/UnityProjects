using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordInput : MonoBehaviour
{
    private WordManager wordManager;

    private void Start() => wordManager = GetComponent<WordManager>();

    void Update()
    {
        foreach (var letter in Input.inputString) 
            wordManager.TypeLetter(letter);
    }
}
