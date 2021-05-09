using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    //private GridBuilder<PathNode> grid;
    public int x;
    public float y;
    public int z;

    public int gCost;
    public int hCost;
    public int fCost;
    public bool isWalkable;
    public bool isWater;
    public int modifier;
    public PathNode previousNode;
    public GameObject placedObject;
    public PathNode(/*GridBuilder<PathNode> _grid, */int _x, int _z, int _modifier)
    {
        //grid = _grid;
        x = _x;
        z = _z;
        modifier = _modifier;
        isWalkable = true;
    }

    public void ChangePlacedObject(GameObject newObject, out GameObject toRemove)
    {
        toRemove = null;
        if (placedObject != null)
            toRemove = placedObject;
        placedObject = newObject;
    }

    public void CalculateFCost() => fCost = gCost + hCost - modifier;
}