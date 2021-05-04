using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public Placeable placeable;

    private void Start()
    {
        PathFindingTest.instance.pathfinding.GetGrid().GetXY(transform.position, out int x, out int y);
        if (placeable == Placeable.wall)
        {
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, y).isWalkable = false;
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, y).modifier = 0;
        }
        else if (placeable == Placeable.path)
        {
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, y).isWalkable = true;
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, y).modifier = 6;
        }
    }

}
