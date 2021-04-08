using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScoreText : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.SetScoreText(GetComponent<TMPro.TMP_Text>());
    }
}
