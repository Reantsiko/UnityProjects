using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Placeable { wall, path}


public class PathFindingTest : MonoBehaviour
{
    public static PathFindingTest instance;
    public Pathfinding pathfinding;
    public Pathfinder unit;
    public Pathfinder unit2;
    public bool debug = true;
    Coroutine coroutine;
    Coroutine coroutine2;
    public GameObject wall = null;
    public GameObject path = null;
    public GameObject selected = null;
    public LayerMask groundMask;
    public GameObject altarPrefab;

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
        var units = FindObjectsOfType<Pathfinder>().ToList();
        units.ForEach(x => x.pathfinding = pathfinding);
        Debug.Log($"Finished Grid Buidling");
        StartCoroutine(ClearUnreachableList());
        SpawnAltar(61 / 2);
    }

    private void SpawnAltar(int middleOfMap)
    {
        Debug.Log(middleOfMap);
        bool isGoodLocation = false;
        var x = middleOfMap;
        var z = middleOfMap;
        //Debug.Log(isGoodLocation);
        while (!isGoodLocation)
        {
            isGoodLocation = CheckPositions(3, 3, ref x, ref z);
            if (!isGoodLocation)
                Debug.Log($"Cannot spawn altar here!");
        }
        if (altarPrefab != null)
        {
            var spawnPos = pathfinding.GetGrid().GetWorldPosition(x, z);
            spawnPos.y = pathfinding.GetGrid().GetGridObject(x, z).y;
            Instantiate(altarPrefab, spawnPos, Quaternion.identity);
            MakeUnWalkable(x, z);
        }
    }

    private void MakeUnWalkable(int xPos, int zPos)
    {
        for (int z = 0; z < 2; z++)
        {
            for (int x = 0; x < 3; x++)
            {
                pathfinding.GetGrid().GetGridObject(xPos + x, zPos + z).isWalkable = false;
            }
        }
    }

    private bool CheckPositions(int xSize, int zSize, ref int startX, ref int startZ)
    {
        int i = 0;
        while (CheckCells(3, 3, startX, startZ) == false)
        {
            Debug.Log($"Trying to place Altar");
            startX += xSize * i;
            startZ += zSize * i;
            Debug.Log(i);
                i++;
        }
        return true;
    }

    private bool CheckCells(int maxX, int maxZ, int xPos, int zPos)
    {
        for (int z = -1; z < maxZ; z++)
            for (int x = 0; x < maxX; x++)
            {
                if (!pathfinding.GetGrid().GetGridObject(xPos + x, zPos + z).isWalkable)
                    return false;
            }
        return true;
    }

    IEnumerator ClearUnreachableList()
    {
        while (true)
        {
            yield return new WaitForSeconds(300f);
            pathfinding.ResetUnreachable();
        }
    }

    // Update is called once per frame
    /*void Update()
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
    }*/

    private Vector3 RayHitPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        if (hit.collider != null)
            return hit.point;
        return -Vector3.one;
    }
}
