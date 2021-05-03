using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;
    public bool isWalkable;
    public PathNode previousNode;
    public PathNode(Grid<PathNode> _grid, int _x, int _y)
    {
        grid = _grid;
        x = _x;
        y = _y;
        isWalkable = true;
    }
    public void CalculateFCost()=> fCost = gCost + hCost;
}