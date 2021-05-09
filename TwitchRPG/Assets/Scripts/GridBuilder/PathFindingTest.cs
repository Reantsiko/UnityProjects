using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Placeable { wall, path}


public class PathFindingTest : MonoBehaviour
{
    public static PathFindingTest instance;
    public Pathfinding pathfinding;
    public UnitPathfinder unit;
    public UnitPathfinder unit2;
    public bool debug = true;
    Coroutine coroutine;
    Coroutine coroutine2;
    public GameObject wall = null;
    public GameObject path = null;
    public GameObject selected = null;
    public LayerMask groundMask;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //StartGridBuilding();
    }

    public void StartGridBuilding()
    {
        var tempList = new List<PathNode>();
        pathfinding = new Pathfinding(61, 61, transform.position, debug);
        for (int x = 0; x < pathfinding.GetGrid().GetWidth(); x++)
        {
            for (int z = 0; z < pathfinding.GetGrid().GetDepth(); z++)
            {
                var pos = pathfinding.GetGrid().GetCenterCell(x, z);
                Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 10000f, groundMask);
                //Debug.Log($"{hit.collider}");
                if (hit.collider != null)
                {
                    pathfinding.GetGrid().GetGridObject(x, z).y = hit.point.y;
                    if (hit.point.y < 1f || hit.point.y > 5f)
                    {
                        pathfinding.GetGrid().GetGridObject(x, z).isWalkable = false;
                        pathfinding.GetGrid().GetGridObject(x, z).isWater = hit.point.y < 1f ? true : false;
                        Debug.Log(false);
                    }
                    else
                        tempList.Add(pathfinding.GetGrid().GetGridObject(x, z));
                    //Debug.Log($"{hit.point.y}");
                }
            }
        }
        //tempList.ForEach(x => Debug.Log($"x: {x.x} y: {x.y} z: {x.z}"));
        Debug.Log($"Finished Grid Buidling");
        unit.pathfinding = pathfinding;
        unit2.pathfinding = pathfinding;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = RayHitPosition();
            unit.StartPathfinding(position);
        }
        if (Input.GetMouseButtonDown(1))
        {
            var position = RayHitPosition();
            unit2.StartPathfinding(position);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wall == null || path == null) return;
            selected = selected == wall ? path : wall;
        }
    }

    private Vector3 RayHitPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        if (hit.collider != null)
            return hit.point;
        return -Vector3.one;
    }
}
