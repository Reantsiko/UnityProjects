using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class WordManager : MonoBehaviour
{
    [Tooltip("The maximum of enemy units that can attack in case of multiple targets having the same word")]
    public int maxAttacks = 3;
    public int scorePerLetter = 10;
    [SerializeField] private TMP_Text comboText = null;
    private bool hasActiveWord = false;
    [SerializeField]private Word activeWord = null;
    [SerializeField] private List<Word> words = new List<Word>();
    [SerializeField] private List<Word> possibleWords = new List<Word>();
    private WordSpawner wordSpawner = null;
    private int combo = 0;

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
        Word word = new Word(WordLists.instance.GetRandomWord(), scorePerLetter, display);
        if (string.IsNullOrEmpty(word.word))
            word.word = "error";
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
        if (hasActiveWord && activeWord.WordTyped(combo))
            CheckForRemovalWord();
    }
    private void HasActiveWord(char letter)
    {
        if (activeWord.GetNextLetter() == letter)
        {
            Combo();
            activeWord.TypeLetter();
        }
        else
        {
            activeWord.AttackPlayer();
            activeWord.ResetWord();
            activeWord = null;
            hasActiveWord = false;
            UpdateComboText(true);
            TypeLetter(letter);
        }
    }

    private void CreatePossibleWords(char letter)
    {
        possibleWords = words.Where(w => w?.GetNextLetter() == letter).ToList();
        possibleWords.ForEach(w => w?.TypeLetter());
        if (possibleWords.Count() > 0)
            Combo();
    }

    private void IterateOverPossibleWords(char letter)
    {
        var wordsToRemove = possibleWords.Where(w => w.GetNextLetter() != letter).ToList();
        var wordsToType = possibleWords.Where(w => w.GetNextLetter() == letter).ToList();

        if (wordsToType.Count() > 0)
            Combo();

        wordsToType.ForEach(w => w?.TypeLetter());
        var typedWords = wordsToType.Where(w => w != null ? w.WordTyped(combo) : true).ToList();
        typedWords.ForEach(w => wordsToRemove.Add(w));
        wordsToRemove.ForEach(w => w?.ResetWord());
        AttackPlayerOnError(wordsToRemove);

        if (wordsToType.Count() == 0 && possibleWords.Count() == 0)
            TypeLetter(letter);
    }

    private void AttackPlayerOnError(List<Word> wordsToRemove)
    {
        bool fireAtPlayer = wordsToRemove.Count == possibleWords.Count;
        if (fireAtPlayer && wordsToRemove.All(w => !w.wordTyped))
            UpdateComboText(true);
        int attacks = 0;
        wordsToRemove.ForEach(w => AttackPlayer(fireAtPlayer, w, ref attacks));
    }

    private void AttackPlayer(bool fireAtPlayer, Word word, ref int attacks)
    {
        if (fireAtPlayer && !word.isMovement && !word.wordTyped && attacks < maxAttacks)
        {
            attacks++;
            word.AttackPlayer();
        }
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

    private void Combo()
    {
        combo++;
        UpdateComboText();
    }

    private void UpdateComboText(bool reset = false)
    {
        if (reset)
            combo = 0;
        comboText.text = $"Combo\nx{combo}";
    }
}
