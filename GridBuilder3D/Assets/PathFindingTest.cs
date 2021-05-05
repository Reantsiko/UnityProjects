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

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(10, 10, debug);
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
                    Debug.DrawLine(new Vector3(path[i].x, 0f, path[i].y) + Vector3.one * .5f, new Vector3(path[i + 1].x, 0f, path[i + 1].y) + Vector3.one * .5f, Color.red, 5f);
                coroutine = StartCoroutine(WaitTillPointReached(path));
            }
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            var position = RayHitPosition();
            Debug.Log(position);
            //pathfinding.GetGrid().GetGridObject(position).isWalkable = !///pathfinding.GetGrid().GetGridObject(mouseWorldPosition).modifier = 6;
            pathfinding.GetGrid().GetXZ(position, out int x, out int z);
            var location = pathfinding.GetGrid().GetCenterCell(x, z);
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
        var finalPos = pathfinding.GetGrid().GetCenterCell(path[path.Count - 1].x, path[path.Count - 1].y);
        while (unit.position != finalPos)
        {
            yield return new WaitForEndOfFrame();
            unit.position = Vector3.MoveTowards(unit.position, pathfinding.GetGrid().GetCenterCell(current.x, current.y), 10f * Time.deltaTime);
            if (unit.position == pathfinding.GetGrid().GetCenterCell(current.x, current.y) && current != path[path.Count - 1])
                current = path[++i];
        }
        Debug.Log($"Exiting coroutine");
        coroutine = null;
    }
}
