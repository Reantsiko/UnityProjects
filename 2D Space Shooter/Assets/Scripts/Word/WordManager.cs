using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public List<Word> words = new List<Word>();
    public List<Word> possibleWords = new List<Word>();
    private WordSpawner wordSpawner;
    public bool hasActiveWord;
    public Word activeWord;

    private void Start()
    {
        wordSpawner = GetComponent<WordSpawner>();
    }

    public void AddMovementWord(Vector3 moveTarget)
    {
        var wordDisplay = wordSpawner.SpawnWord(true, moveTarget);
        Word word = new Word(WordGenerator.GetRandomMovementWord(), wordDisplay, moveTarget, true);
        Debug.Log(word.word);
        words.Add(word);
        wordDisplay.SetWord(word.word, word);
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
        var temp = new List<Word>();
        foreach (var word in possibleWords)
        {
            if (word.GetNextLetter() == letter)
            {
                word.TypeLetter();
                if (word.WordTyped())
                {
                    temp.Add(word);
                    words.Remove(word);
                }
            }
            else
            {
                word.ResetWord();
                temp.Add(word);
            }
        }

        foreach (var word in temp)
            possibleWords.Remove(word);
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
