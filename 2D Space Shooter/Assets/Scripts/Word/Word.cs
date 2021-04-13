using UnityEngine;
[System.Serializable]
public class Word
{
    public string word;
    public bool isMovement;
    public Vector3 moveTarget;
    public bool wordTyped;
    public int scorePerLetter;
    [SerializeField] private int typeIndex;
    [SerializeField] private WordDisplay display;
    public Word(string _word, WordDisplay _wordDisplay, Vector3 _moveTarget, bool _isMovement = false)
    {
        word = _word;
        display = _wordDisplay;
        isMovement = _isMovement;
        moveTarget = _moveTarget;
        
        wordTyped = false;
        display.SetWord(word, this);
        typeIndex = 0;
    }
    public Word(string _word, int _scorePerLetter, WordDisplay _wordDisplay, bool _isMovement = false)
    {
        word = _word;
        display = _wordDisplay;
        scorePerLetter = _scorePerLetter;
        isMovement = _isMovement;
        display.SetWord(word, this);
        typeIndex = 0;
    }

    public char GetNextLetter()
    {
        return typeIndex < word.Length ? word[typeIndex] : ' ';
    }

    public void TypeLetter()
    {
        if (wordTyped) return;

        typeIndex++;
        if (display != null)
            display.TypeLetter(word, typeIndex);
    }

    public void ResetWord()
    {
        if (wordTyped) return;

        typeIndex = 0;
        if (display != null)
            display.TypeLetter(word, typeIndex);
    }

    public bool WordTyped(int currentCombo)
    {
        if (wordTyped) return wordTyped;

        wordTyped = typeIndex >= word.Length;

        if (wordTyped)
        {
            if (isMovement)
            {
                PlayerMovement.instance.SetMoveTarget(moveTarget);
                display.RemoveWord();
            }
            else
            {
                GameManager.instance.UpdateScore(scorePerLetter * typeIndex + currentCombo);
                if (display)
                    PlayerAttack.instance.Fire(display.transform);
            }
        }
        return wordTyped;
    }

    public void AttackPlayer()
    {
        if (wordTyped) return;

        if (display != null)
            display.AttackPlayer();
    }
}
