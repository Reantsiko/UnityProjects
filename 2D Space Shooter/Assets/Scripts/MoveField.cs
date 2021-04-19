using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MoveField : MonoBehaviour
{
    public static MoveField instance = null;
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;
    [SerializeField] private float cellSize = 1;
    [SerializeField] private bool is2D = false;
    [SerializeField] private bool debug = false;
    [SerializeField] private Transform canvas = null;
    [SerializeField] private GameObject tmpTextPrefab = null;
    [SerializeField] private float textSize = .25f;
    public Grid<Vector3> moveableField = null;
    private WordManager wordManager;

    private TMP_Text CreateTextField(float textSize)
    {
        var text = Instantiate(tmpTextPrefab, canvas);
        var textComponent = text.GetComponent<TMP_Text>();
        textComponent.fontSize = textSize;
        return textComponent;
    }

    private void Start()
    {
        instance = this;
        wordManager = FindObjectOfType<WordManager>();
        moveableField = new Grid<Vector3>(width, height, cellSize, transform.position, is2D, debug, default);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < 2; y++)
            {
                moveableField.SetGridObject(x, y, moveableField.GetCenterOfCell2D(x, y));
                wordManager.AddMovementWord(moveableField.GetCenterOfCell2D(x, y));
                //var obj = new GridObject(CreateTextField(textSize), moveableField.GetCenterOfCell2D(x, y), /*string.Empty*/"test");
                //moveableField.SetGridObject(x, y, obj.GetPos());
            }
    }
}

public class GridObject
{
    private TMP_Text textField;
    private Vector3 pos;
    private string word;
    public GridObject(TMP_Text textField, Vector3 pos, string word)
    {
        this.textField = textField;
        this.pos = pos;
        this.word = word;
        this.textField.transform.position = pos;
        SetText(word);
    }
    public GridObject() { }
    public TMP_Text GetTextField() => textField;
    public Vector3 GetPos() => pos;
    public string GetWord() => word;
    public void SetText(string toSet) => textField.text = toSet;
    public void SetWord(string toSet) => word = toSet;
}



