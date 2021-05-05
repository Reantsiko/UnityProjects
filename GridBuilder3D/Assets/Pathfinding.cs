using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private GridBuilder<PathNode> grid;
    public Pathfinding(int width, int height, bool debug)
    {
        grid = new GridBuilder<PathNode>(width, height, 1f, Vector3.zero, debug, (GridBuilder<PathNode> g, int x, int y) => new PathNode(g, x, y, 0));
    }
    public GridBuilder<PathNode> GetGrid() => grid;

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        var startNode = grid.GetGridObject(startX, startY);
        var endNode = grid.GetGridObject(endX, endY);
        if (startNode == null || endNode == null)
            return null;
        var openList = new List<PathNode> { startNode };
        var closedList = new List<PathNode>();

        PreparePathCalculation(endNode, ref startNode);
        return CalculatePath(endNode, openList, closedList);        
    }

    private List<PathNode> CalculatePath(PathNode endNode, List<PathNode> openList, List<PathNode> closedList)
    {
        while (openList.Count > 0)
        {
            var currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
                return CreatePath(endNode);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            CheckNeighbours(currentNode, endNode, ref openList, ref closedList);
        }
        return null;
    }

    private void PreparePathCalculation(PathNode endNode,ref PathNode startNode)
    {
        for (int x = 0; x < grid.GetWidth(); x++)
            for (int y = 0; y < grid.GetDepth(); y++)
            {
                var pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.hCost = 0;
                pathNode.CalculateFCost();
                pathNode.previousNode = null;
            }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();
    }

    private void CheckNeighbours(PathNode currentNode, PathNode endNode, ref List<PathNode> openList, ref List<PathNode> closedList)
    {
        foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
        {
            if (closedList.Contains(neighbourNode))
                continue;
            if (!neighbourNode.isWalkable)
            {
                closedList.Add(neighbourNode);
                continue;
            }
            int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
            if (tentativeGCost < neighbourNode.gCost)
            {
                neighbourNode.previousNode = currentNode;
                neighbourNode.gCost = tentativeGCost;
                neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                neighbourNode.CalculateFCost();

                if (!openList.Contains(neighbourNode))
                    openList.Add(neighbourNode);
            }
        }
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        if (currentNode.x - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (currentNode.y - 1 >= 0) 
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetDepth()) 
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (currentNode.y - 1 >= 0) 
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetDepth()) 
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        if (currentNode.y - 1 >= 0) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        if (currentNode.y + 1 < grid.GetDepth()) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private PathNode GetNode(int x, int y) => grid.GetGridObject(x, y);

    private List<PathNode> CreatePath(PathNode endNode)
    {
        var currentNode = endNode;
        List<PathNode> path = new List<PathNode>();
        while (currentNode.previousNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.previousNode;
        }
        path.Add(currentNode);
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining - b.modifier;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        => pathNodeList.Find(pathNode => pathNode.fCost == pathNodeList.Min(minCost => minCost.fCost));
}