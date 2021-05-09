using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathfinder : MonoBehaviour
{
    public Pathfinding pathfinding;
    Coroutine coroutine;

    List<PathNode> PathFind(Vector3 position, Vector3 unitPosition)
    {
        pathfinding.GetGrid().GetXZ(position, out int x, out int z);
        pathfinding.GetGrid().GetXZ(unitPosition, out int sx, out int sz);
        return pathfinding.FindPath(sx, sz, x, z);
    }

    void OnPathReceived(object pathData)
    {
        var path = (List<PathNode>)pathData;
        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
                Debug.DrawLine(new Vector3(path[i].x, 0f, path[i].z) + Vector3.one * .5f, new Vector3(path[i + 1].x, 0f, path[i + 1].z) + Vector3.one * .5f, Color.red, 5f);

            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(WaitTillPointReached(path));
        }
    }
    public void StartPathfinding(Vector3 position)
    {
        var unitPosition = transform.position;
        ThreadedDataRequester.RequestData(() => PathFind(position, unitPosition), OnPathReceived);
    }

    IEnumerator WaitTillPointReached(List<PathNode> path)
    {
        int i = 0;
        var current = path[i];
        var finalPos = GetPosition(path[path.Count - 1].x, path[path.Count - 1].z);
        while (transform.position != finalPos)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.MoveTowards(transform.position, GetPosition(current.x, current.z), 10f * Time.deltaTime);
            if (transform.position == GetPosition(current.x, current.z) && current != path[path.Count - 1])
                current = path[++i];
        }
        coroutine = null;
    }
    private Vector3 GetPosition(int x, int z)
    {
        var centerPos = pathfinding.GetGrid().GetCenterCell(x, z);
        centerPos.y = pathfinding.GetGrid().GetGridObject(x, z).y;
        //Debug.Log($"{centerPos}");
        return centerPos;
    }
}
