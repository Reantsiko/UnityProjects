using UnityEngine;
[System.Serializable]
public class Word
{
    public string word;
    public WordType wordType;
    public PowerUpType powerUpType;
    public Vector3 moveTarget;
    public bool wordTyped;
    public int scorePerLetter;
    [SerializeField] private int typeIndex;
    [SerializeField] private WordDisplay display;
    public Word(string _word, WordDisplay _wordDisplay, Vector3 _moveTarget, WordType _wordType)
    {
        word = _word;
        display = _wordDisplay;
        wordType = _wordType;
        moveTarget = _moveTarget;
        
        wordTyped = false;
        display.SetWord(word, this);
        typeIndex = 0;
    }
    public Word(string _word, WordDisplay _wordDisplay, WordType _wordType, Vector3 _moveTarget, int _scorePerLetter = 0)
    {
        word = _word;
        display = _wordDisplay;
        scorePerLetter = _scorePerLetter;
        wordType = _wordType;
        moveTarget = _moveTarget;
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
            display.TypeLetter(word, typeIndex, wordType);
    }

    public void ResetWord()
    {
        if (wordTyped) return;

        typeIndex = 0;
        if (display != null)
            display.TypeLetter(word, typeIndex, wordType);
    }

    public bool WordTyped(int currentCombo)
    {
        if (wordTyped) return wordTyped;

        wordTyped = typeIndex >= word.Length;

        if (wordTyped)
        {
            if (wordType == WordType.Movement)
                MovementWord();
            else if (wordType == WordType.Enemy)
                EnemyWord(currentCombo);
            else if (wordType == WordType.PowerUp)
                PowerUpWord();
        }
        return wordTyped;
    }

    private void MovementWord()
    {
        PlayerMovement.instance.SetMoveTarget(moveTarget);
        MoveField.instance.moveableField.SetGridObject(moveTarget, false);
        display.RemoveWord();
    }

    private void EnemyWord(int currentCombo)
    {
        GameManager.instance.UpdateScore(scorePerLetter * typeIndex + currentCombo);
        if (display)
            PlayerAttack.instance.Fire(display.transform);
    }

    private void PowerUpWord()
    {
        if (powerUpType == PowerUpType.Bomb)
            PowerUpUI.instance.UseBomb();
        else if (powerUpType == PowerUpType.Shield)
            PowerUpUI.instance.UseShield();
        display.TypeLetter(word, 0, wordType);
    }

    public void Bombed()
    {
        if (wordType != WordType.Enemy) return;

        GameManager.instance.UpdateScore(scorePerLetter * word.Length);
        if (display)
            PlayerAttack.instance.Fire(display.transform);
    }

    public void AttackPlayer()
    {
        if (wordTyped) return;

        if (display != null)
            display.AttackPlayer();
    }
}
