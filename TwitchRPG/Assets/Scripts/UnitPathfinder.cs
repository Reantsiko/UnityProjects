using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitPathfinder : MonoBehaviour
{
    public PJob job = PJob.explorer;
    public Pathfinding pathfinding;
    public bool doesJob = false;
    public float moveSpeed = 30f;
    Coroutine coroutine;
    Coroutine jobRoutine;
    PathNode target;
    List<PathNode> PathFind(Vector3 position, Vector3 unitPosition)
    {
        pathfinding.GetGrid().GetXZ(position, out int x, out int z);
        pathfinding.GetGrid().GetXZ(unitPosition, out int sx, out int sz);
        return pathfinding.FindPath(sx, sz, x, z);
    }

    private void Start()
    {
        pathfinding = PathFindingTest.instance.pathfinding;
    }

    void OnPathReceived(object pathData)
    {
        var path = (List<PathNode>)pathData;
        if (path != null)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(WaitTillPointReached(path));
        }
        else
        {
            doesJob = false;
            Debug.Log($"{gameObject.name}, no path found!");
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
            transform.position = Vector3.MoveTowards(transform.position, GetPosition(current.x, current.z), moveSpeed * Time.deltaTime);
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (job == PJob.explorer)
            {
                doesJob = true;
                if (coroutine != null)
                    StopCoroutine(coroutine);
                StartCoroutine(ExploreMap());
            }
        }
    }

    private IEnumerator ExplorePosition(PathNode target)
    {
        if (target.GetIsUnexploredAndReachable())
        {
            for (int i = 0; i < 3; i++)
                yield return new WaitForSeconds(.5f);
            if (job == PJob.explorer)
            {
                var p = GetComponent<Player>();
                if (p != null)
                    p.playerJob.GainXP(5);
                target.SetIsExplored();
            }
        }
        else
            Debug.Log($"This already has been explored!");
        jobRoutine = null;
    }

    private IEnumerator ExploreMap()
    {
        while (doesJob && job == PJob.explorer)
        {
            if (jobRoutine == null && coroutine == null)
            {
                if (pathfinding.GetGrid().GetGridObject(transform.position).GetIsUnexploredAndReachable())
                {
                    target = pathfinding.GetGrid().GetGridObject(transform.position);
                    jobRoutine = StartCoroutine(ExplorePosition(pathfinding.GetGrid().GetGridObject(transform.position)));
                }
                else if (CheckNeighbours() == false)
                    if (SearchRadius() == false)
                        doesJob = false;
            }
            yield return new WaitForSeconds(.3f);
        }
        //Debug.Log($"{gameObject.name} no more nodes to explore!");
    }

    private bool CheckNeighbours()
    {
        target = GetNeighbours(transform.position);
        if (target == null)
            return false;
        StartPathfinding(pathfinding.GetGrid().GetCenterCell(target.x, target.z));
        return true;
    }

    private bool SearchRadius()
    {
        for (int i = 2; i <= 5; i++)
        {
            var foundNode = pathfinding.FindInRadius(pathfinding.GetGrid().GetGridObject(transform.position), i);
            if (foundNode != null && target != foundNode)
            {
                target = foundNode;
                StartPathfinding(pathfinding.GetGrid().GetCenterCell(foundNode.x, foundNode.z));
                //Debug.Log($"{gameObject.name} has found a node within the radius!");
                return true;
            }
        }
        return false;
    }

    private PathNode GetNeighbours(Vector3 playerPos)
    {
        var neighbours = pathfinding.GetNeighbours(pathfinding.GetGrid().GetGridObject(playerPos));
        var toReturn = neighbours.Find(x => x.GetIsUnexploredAndReachable() && !x.unreachable && x.isWalkable);
        return toReturn;
    }
}
