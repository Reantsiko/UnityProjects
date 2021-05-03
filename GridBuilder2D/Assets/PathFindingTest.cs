using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingTest : MonoBehaviour
{
    private Pathfinding pathfinding;
    public Transform unit;
    public float moveSpeed = 10f;
    Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetGrid().GetXY(unit.position, out int sx, out int sy);
            var path = pathfinding.FindPath(sx, sy, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) + Vector3.one * .5f, new Vector3(path[i + 1].x, path[i + 1].y) + Vector3.one * .5f, Color.red, 5f);
                coroutine = StartCoroutine(WaitTillPointReached(path));
            }
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathfinding.GetGrid().GetGridObject(mouseWorldPosition).isWalkable = !pathfinding.GetGrid().GetGridObject(mouseWorldPosition).isWalkable;
        }

        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
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
