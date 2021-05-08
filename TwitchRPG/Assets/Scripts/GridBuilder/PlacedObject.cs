using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public Placeable placeable;

    private void Start()
    {
        PathFindingTest.instance.pathfinding.GetGrid().GetXZ(transform.position, out int x, out int z);
        if (placeable == Placeable.wall)
        {
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, z).isWalkable = false;
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, z).modifier = 0;
        }
        else if (placeable == Placeable.path)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.49f, transform.position.z);
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, z).isWalkable = true;
            PathFindingTest.instance.pathfinding.GetGrid().GetGridObject(x, z).modifier = 6;            
        }
    }

}
