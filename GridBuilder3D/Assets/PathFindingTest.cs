using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Placeable { wall, path}


public class PathFindingTest : MonoBehaviour
{
    public static PathFindingTest instance;
    public Pathfinding pathfinding;
    public Transform unit;
    public bool debug = true;
    Coroutine coroutine;
    public GameObject wall = null;
    public GameObject path = null;
    public GameObject selected = null;
    public LayerMask groundMask;

    private void Awake()
    {
        instance = this;
    }

    public void StartGridBuilding()
    {
        pathfinding = new Pathfinding(10, 10, debug);
        for (int x = 0; x < pathfinding.GetGrid().GetWidth(); x++)
        {
            for (int z = 0; z < pathfinding.GetGrid().GetDepth(); z++)
            {
                var pos = pathfinding.GetGrid().GetCenterCell(x, z);
                Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 1000f, groundMask);
                if (hit.collider != null)
                {
                    pathfinding.GetGrid().GetGridObject(x, z).y = hit.point.y;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            //Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var position = RayHitPosition();
            pathfinding.GetGrid().GetXZ(position, out int x, out int z);
            pathfinding.GetGrid().GetXZ(unit.position, out int sx, out int sz);
            var path = pathfinding.FindPath(sx, sz, x, z);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                    Debug.DrawLine(new Vector3(path[i].x, 0f, path[i].z) + Vector3.one * .5f, new Vector3(path[i + 1].x, 0f, path[i + 1].z) + Vector3.one * .5f, Color.red, 5f);
                coroutine = StartCoroutine(WaitTillPointReached(path));
            }
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            var position = RayHitPosition();
            pathfinding.GetGrid().GetXZ(position, out int x, out int z);
            
            if (x < 0 || z < 0) return;
            var location = GetPosition(x, z);//pathfinding.GetGrid().GetCenterCell(x, z);
            if (selected != null && x >= 0 && z >= 0)
            {
                var instance = Instantiate(selected, location, Quaternion.identity);
                pathfinding.GetGrid().GetGridObject(location).ChangePlacedObject(instance, out GameObject toRemove);
                if (toRemove != null)
                    Destroy(toRemove);   
            }
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

    IEnumerator WaitTillPointReached(List<PathNode> path)
    {
        int i = 0;
        var current = path[i];
        var finalPos = GetPosition(path[path.Count - 1].x, path[path.Count - 1].z);
        while (unit.position != finalPos)
        {
            yield return new WaitForEndOfFrame();
            unit.position = Vector3.MoveTowards(unit.position, GetPosition(current.x, current.z), 10f * Time.deltaTime);
            if (unit.position == GetPosition(current.x, current.z) && current != path[path.Count - 1])
                current = path[++i];
        }
        coroutine = null;
    }

    private Vector3 GetPosition(int x, int z)
    {
        var centerPos = pathfinding.GetGrid().GetCenterCell(x, z);
        centerPos.y = pathfinding.GetGrid().GetGridObject(x, z).y;
        return centerPos;
    }
}
