﻿using UnityEngine;
using TMPro;
public class WordDisplay : MonoBehaviour
{
    public TMP_Text text = null;
    public WordManager wordManager;
    public Word word;
    [SerializeField] private EnemyAttack enemyAttack;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();
        wordManager = FindObjectOfType<WordManager>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    public void SetWord(string toSet, Word _word)
    {
        if (text == null) return;

        if (_word.wordType == WordType.Enemy)
            text.text = $"<color=red>{toSet}</color>";
        else if (_word.wordType == WordType.Movement)
            text.text = $"<color=orange>{toSet}</color>";
        else
            text.text = $"<color=white>{toSet}</color>";
        word = _word;
    }
    public void TypeLetter(string word, int index, WordType wordType)
    {
        switch (wordType)
        {
            case WordType.Enemy:
                text.text = $"<color=green>{word.Substring(0, index)}</color><color=red>{word.Substring(index)}</color>";
                break;
            case WordType.PowerUp:
                text.text = $"<color=green>{word.Substring(0, index)}</color><color=white>{word.Substring(index)}</color>";
                break;
            case WordType.Movement:
                text.text = $"<color=green>{word.Substring(0, index)}</color><color=orange>{word.Substring(index)}</color>";
                break;
        }
        
    }
    public void RemoveWordFromList() => wordManager.RemoveWord(word);
    public void AttackPlayer()
    {
        if (enemyAttack != null)
        {
            enemyAttack.FireAtPlayer();
        }
    }
    public void RemoveWord() => Destroy(gameObject);
}
