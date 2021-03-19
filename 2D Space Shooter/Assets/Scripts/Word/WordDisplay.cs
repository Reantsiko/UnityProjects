using UnityEngine;
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
        text.text = $"<color=red>{toSet}</color>";
        word = _word;
    }

    public void TypeLetter(string word, int index)
    {
        text.text = $"<color=green>{word.Substring(0, index)}</color><color=red>{word.Substring(index)}</color>";
    }

    public void RemoveWordFromList()
    {
        wordManager.RemoveWord(word);
    }

    public void AttackPlayer()
    {
        enemyAttack.FireAtPlayer();
    }

    public void RemoveWord() => Destroy(gameObject);
}
