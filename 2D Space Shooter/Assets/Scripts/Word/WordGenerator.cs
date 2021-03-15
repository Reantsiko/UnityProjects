using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WordGenerator
{
    private static string[] wordList = { "sidewalk", "three", "there", "periodic", "economic" };
    private static string[] wordsForMovement = {"clap", "obtain", "depend", "disapprove", "tick", "delay", "decay", "kill", "deceive", "protect", "bleach", "look", "inform", "share", "rush","compete","possess","change","seal","milk"};
    public static string GetRandomWord() => wordList[Random.Range(0, wordList.Length)];
    public static string GetRandomMovementWord() => wordsForMovement[Random.Range(0, wordsForMovement.Length)];
}
