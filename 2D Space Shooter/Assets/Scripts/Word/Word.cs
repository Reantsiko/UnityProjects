using UnityEngine;
[System.Serializable]
public class Word
{
    public string word;
    public bool isMovement;
    public Vector3 moveTarget;
    [SerializeField] private int typeIndex;
    [SerializeField] private WordDisplay display;
    public Word(string _word, WordDisplay _wordDisplay, Vector3 _moveTarget, bool _isMovement = false)
    {
        word = _word;
        display = _wordDisplay;
        isMovement = _isMovement;
        moveTarget = _moveTarget;
        display.SetWord(word, this);
        typeIndex = 0;
    }
    public Word(string _word, WordDisplay _wordDisplay, bool _isMovement = false)
    {
        word = _word;
        display = _wordDisplay;
        isMovement = _isMovement;
        display.SetWord(word, this);
        typeIndex = 0;
    }

    public char GetNextLetter()
    {
        return word[typeIndex];
    }

    public void TypeLetter()
    {
        typeIndex++;
        display.TypeLetter(word, typeIndex);
    }

    public void ResetWord()
    {
        typeIndex = 0;
        display.TypeLetter(word, typeIndex);
    }

    public bool WordTyped()
    {
        bool wordTyped = typeIndex >= word.Length;

        if (wordTyped)
        {
            if (isMovement)
                PlayerMovement.instance.SetMoveTarget(moveTarget);
            display.RemoveWord();
        }

        return wordTyped;
    }
}
