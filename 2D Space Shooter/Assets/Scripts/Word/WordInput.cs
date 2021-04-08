using UnityEngine;
using System.Linq;
public class WordInput : MonoBehaviour
{
    private WordManager wordManager;

    private void Start() => wordManager = GetComponent<WordManager>();

    void Update() => Input.inputString.ToList().ForEach(l => wordManager.TypeLetter(l));
}
