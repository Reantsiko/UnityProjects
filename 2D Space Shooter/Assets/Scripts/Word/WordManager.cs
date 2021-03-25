using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    [Tooltip("The maximum of enemy units that can attack in case of multiple targets having the same word")]
    public int maxAttacks = 3;
    public int scorePerLetter = 10;
    private bool hasActiveWord = false;
    private Word activeWord = null;
    private List<Word> words = new List<Word>();
    private List<Word> possibleWords = new List<Word>();
    private WordSpawner wordSpawner = null;

    private void Awake() => wordSpawner = GetComponent<WordSpawner>();

    public void RemoveWord(Word toRemove) => words.Remove(toRemove);

    public void AddMovementWord(Vector3 moveTarget)
    {
        var wordDisplay = wordSpawner.SpawnWord(true, moveTarget);
        Word word = new Word(WordGenerator.GetRandomMovementWord(), wordDisplay, moveTarget, true);
        words.Add(word);
        wordDisplay.SetWord(word.word, word);
    }

    public void AddEnemyWord(WordDisplay display)
    {
        Word word = new Word(WordGenerator.GetRandomWord(), scorePerLetter, display);
        words.Add(word);
        display.SetWord(word.word, word);
    }

    public void TypeLetter(char letter)
    {
        if (hasActiveWord)
            HasActiveWord(letter);
        else
        {
            if (possibleWords.Count == 0)
                CreatePossibleWords(letter);
            else
                IterateOverPossibleWords(letter);
            if (possibleWords.Count == 1)
                SetActiveWord();
        }
        if (hasActiveWord && activeWord.WordTyped())
            CheckForRemovalWord();
    }
    private void HasActiveWord(char letter)
    {
        if (activeWord.GetNextLetter() == letter)
            activeWord.TypeLetter();
        else
        {
            activeWord.ResetWord();
            activeWord = null;
            hasActiveWord = false;
        }
    }

    private void CreatePossibleWords(char letter)
    {
        foreach (var word in words)
        {
            if (word.GetNextLetter() == letter)
            {
                possibleWords.Add(word);
                word.TypeLetter();
                continue;
            }
        }
    }

    private void IterateOverPossibleWords(char letter)
    {
        var wordsToRemove = new List<Word>();
        foreach (var word in possibleWords)
        {
            if (word == null)
                continue;
            if (word.GetNextLetter() == letter)
            {
                word.TypeLetter();
                if (word.WordTyped())
                {
                    wordsToRemove.Add(word);
                    words.Remove(word);
                }
            }
            else
            {
                word.ResetWord();
                wordsToRemove.Add(word);
            }
        }
        AttackPlayerOnError(wordsToRemove);
    }

    private void AttackPlayerOnError(List<Word> wordsToRemove)
    {
        bool fireAtPlayer = wordsToRemove.Count == possibleWords.Count;
        int attacks = 0;
        foreach (var word in wordsToRemove)
        {
            if (fireAtPlayer && !word.isMovement && !word.wordTyped && attacks < maxAttacks)
            {
                attacks++;
                word.AttackPlayer();
            }
            possibleWords.Remove(word);
        }
    }

    private void CheckForRemovalWord()
    {
        hasActiveWord = false;
        words.Remove(activeWord);
        activeWord = null;
    }

    private void SetActiveWord()
    {
        hasActiveWord = true;
        activeWord = possibleWords[0];
        possibleWords.Clear();
    }
}
