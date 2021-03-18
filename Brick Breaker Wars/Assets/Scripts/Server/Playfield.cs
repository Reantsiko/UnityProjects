using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayField
{    
    /*
     * Variables
    */
    private float _width;
    private float _height;
    private float _blockWidth;
    private float _blockHeight;
    private Vector3 _originPosition;
    public Block[,] _fieldArray;
    public List<Block> _blockList;
    private bool _debug;
    /*
    * Public Methods
    */
    /*
     * Initializes the playfield, bool debug allows you to see the field in the  scene view on Unity.
    */
    public PlayField(float width, float height, float blockWidth, float blockHeight, Vector3 originPosition, bool debug)
    {
        _width = width / blockWidth;
        _height = height / blockHeight;
        _blockWidth = blockWidth;
        _blockHeight = blockHeight;
        _originPosition = originPosition;
        _debug = debug;
        _fieldArray = new Block[(int)_width, (int)_height];
        _blockList = new List<Block>();
        if (_debug)
            VisualizeGrid();
    }

    /*
     * Sets the value in _fieldArray at position [x,y]
     * Value is Block or null.
    */
    [Server]
    public void SetValue(int x, int y, Block value)
    {
        if (_fieldArray != null && x >= 0 && y >= 0 && x < _width && y < _height)
        {
            _fieldArray[x, y] = value;
        }
    }
    /*
     * Probably obsolete code, was created during earlier version.
     * Disables or activates object in the fieldArray at position [x,y]
     */
    [Server]
    public void ChangeActive(int x, int y, bool value)
    {
        if (_fieldArray != null && x >= 0 && y >= 0 && x < _width && y < _height)
            _fieldArray[x, y].gameObject.SetActive(value);
    }
    [Server]
    public void AddBlockToList(Block toAdd)
    {
        _blockList.Add(toAdd);
    }
    [Server]
    public void RemoveBlockFromList(Block toRemove)
    {
        _blockList.Remove(toRemove);
    }
    /*
     * Private Methods
    */
    /*
     * Draws the grid from top to bottom
    */
    private void VisualizeGrid()
    {
        for (float x = 0; x < _fieldArray.GetLength(0); x++)
        {
            for (float y = 0; y < _fieldArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPosition2D(x, -y), GetWorldPosition2D(x, -y - 1), Color.red, 100f);
                Debug.DrawLine(GetWorldPosition2D(x, -y), GetWorldPosition2D(x + 1, -y), Color.red, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition2D(0, -_height), GetWorldPosition2D(_width, -_height), Color.red, 100f);
        Debug.DrawLine(GetWorldPosition2D(_width, 0), GetWorldPosition2D(_width, -_height), Color.red, 100f);
    }
    /*
     * Setter and Getter Methods
    */
    public Vector3 GetWorldPosition2D(float x, float y)
    {
        return new Vector3(x * _blockWidth, y * _blockHeight) + _originPosition;
    }
    public float GetBlockWidth() { return _blockWidth; }
    public float GetBlockHeight() { return _blockHeight; }
    public float GetWidth() { return _width; }
    public float GetHeight() { return _height; }
    [Server]
    public void SetFieldArray(Block[,] toSet) { _fieldArray = toSet; }
}