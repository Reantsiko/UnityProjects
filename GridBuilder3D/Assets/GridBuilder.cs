using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridBuilder<TGridObject> {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int z;
    }

    private int width;
    private int depth;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private bool debug;

    public GridBuilder(int _width, int _depth, float _cellSize, Vector3 _originPosition, bool _debug, Func<GridBuilder<TGridObject>, int, int, TGridObject> createGridObject) {
        width = _width;
        depth = _depth;
        cellSize = _cellSize;
        originPosition = _originPosition;
        debug = _debug;

        gridArray = new TGridObject[width, depth];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        if (debug)
            ShowDebug();
    }

    private void ShowDebug()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 1000f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, depth), GetWorldPosition(width, depth), Color.white, 1000f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, depth), Color.white, 1000f);
    }

    public int GetWidth() => width;
    public int GetDepth() => depth;
    public float GetCellSize() => cellSize;
    public Vector3 GetWorldPosition(int x, int z) => new Vector3(x, 0f, z) * cellSize + originPosition;
    public Vector3 GetCenterCell(int x, int z) => GetWorldPosition(x, z) + Vector3.one * (cellSize / 2);
    public void GetXZ(Vector3 worldPosition, out int x, out int z) 
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value) 
    {
        if (x >= 0 && z >= 0 && x < width && z < depth) 
        {
            gridArray[x, z] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }
    }

    public void TriggerGridObjectChanged(int x, int z) 
    {
        if (OnGridObjectChanged != null) 
            OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) 
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z) 
    {
        if (x >= 0 && z >= 0 && x < width && z < depth)
            return gridArray[x, z];
        else
            return default(TGridObject);
    }

    public TGridObject GetGridObject(Vector3 worldPosition) 
    {
        GetXZ(worldPosition, out int x, out int z);
        return GetGridObject(x, z);
    }
}
