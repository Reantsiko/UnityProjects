using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MoveField : MonoBehaviour
{
    public static MoveField instance = null;
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;
    [SerializeField] private int currAllowedWidth = 2;
    [SerializeField] private float cellSize = 1;
    [SerializeField] private bool is2D = false;
    [SerializeField] private bool debug = false;
    [SerializeField] private Transform canvas = null;
    [SerializeField] private GameObject tmpTextPrefab = null;
    [SerializeField] private float textSize = .25f;
    [SerializeField] private Vector2 playerPos;
    [SerializeField] private float waitTime = 15f;
    public Grid<bool> moveableField = null;
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
        moveableField = new Grid<bool>(width, height, cellSize, transform.position, is2D, debug, default);
        StartCoroutine(WaitForSpawnNewMovementWord());
    }

    private bool CheckPlayerPos(int x, int y)
    {
        if (PlayerMovement.instance.transform.position == moveableField.GetCenterOfCell2D(x, y))
        {
            playerPos.x = x;
            playerPos.y = y;
            return true;
        }
        return false;
    }

    private IEnumerator WaitForSpawnNewMovementWord()
    {
        while (GameManager.instance.playerLives >= 0)
        {
            SpawnMovementWord();
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnMovementWord()
    {
        /*var xPos = Random.Range(0, width);
        var yPos = Random.Range(0, currAllowedHeight);*/
        /*if (xPos == playerPos.x && yPos == playerPos.y)
        {
            SpawnMovementWord();
            return;
        }*/
        /*var temp = moveableField.GetGridObject(xPos, yPos);
        Debug.Log(temp);
        if (moveableField.GetGridObject(xPos, yPos) == default)*/
        var x = Random.Range(0, currAllowedWidth);
        var y = Random.Range(0, height);
        if (!CheckPlayerPos(x, y) && moveableField.GetGridObject(x, y) == false)
        {
            wordManager.AddMovementWord(moveableField.GetCenterOfCell2D(x, y));
            moveableField.SetGridObject(x, y, true);
        }
    }
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var xPos = Random.Range(0, width);
            var yPos = Random.Range(0, currAllowedHeight);
            if (!CheckPlayerPos(xPos, yPos) && moveableField.GetGridObject(xPos, yPos) == false)
            {
                wordManager.AddMovementWord(moveableField.GetCenterOfCell2D(xPos, yPos));
                moveableField.SetGridObject(xPos, yPos, true);
            }
        }
    }*/
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



