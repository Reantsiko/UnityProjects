using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private bool is2D;
    private bool debug;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, bool is2D, bool debug, Func<TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.is2D = is2D;
        this.debug = debug;
        gridArray = new TGridObject[width, height];

        if (debug)
        {
            if (is2D)
                DrawGrid(GetWorldPosition2D);
            else
                DrawGrid(GetWorldPosition3D);
        }
        /*for (int x = 0; x < gridArray.GetLength(0); x++)
            for (int y = 0; y < gridArray.GetLength(1); y++)
                gridArray[x, y] = createGridObject();*/
    }

    public int GetWidth() => width;
    public int GetHeight() => height;

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (IsValidPosition(x, y))
            gridArray[x, y] = value;
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (IsValidPosition(x, y))
            return gridArray[x, y];
        return default;
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    private void DrawGrid(Func<int, int, Vector3> WorldPosition)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(WorldPosition(x, y), WorldPosition(x, y + 1), Color.red, 100f);
                Debug.DrawLine(WorldPosition(x, y), WorldPosition(x + 1, y), Color.red, 100f);
            }
        }
        Debug.DrawLine(WorldPosition(0, height), WorldPosition(width, height), Color.red, 100f);
        Debug.DrawLine(WorldPosition(width, 0), WorldPosition(width, height), Color.red, 100f);
    }

    public Vector3 GetWorldPosition3D(int x, int y)
    {
        return new Vector3(x, 0f, y) * cellSize + originPosition;
    }

    public Vector3 GetWorldPosition2D(int x, int y)
    {
        return new Vector3(x * cellSize, y * cellSize, 0f)  + originPosition;
    }

    public Vector3 GetCenterOfCell2D(int x, int y)
    {
        var worldPos = GetWorldPosition2D(x, y);
        var halfCellSize = cellSize / 2;
        worldPos.x += halfCellSize;
        worldPos.y += halfCellSize;
        return worldPos;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    private bool IsValidPosition(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return true;
        return false;
    }
}