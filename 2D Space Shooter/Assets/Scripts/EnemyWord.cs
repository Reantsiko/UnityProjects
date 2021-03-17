using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnemyWord : MonoBehaviour
{
    public WordDisplay display = null;
    private void Start()
    {
        if (display == null)
            display = GetComponent<WordDisplay>();
        FindObjectOfType<WordManager>().AddEnemyWord(display);
    }
}
