using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder 
{
    Dictionary<Vector2Int, Node> nodeMap = new();
    public Pathfinder()
    {
        for(int x = 0; x < MapTools.gameMap.GetLength(0); x++)
        {
            for(int y = 0; y <  MapTools.gameMap.GetLength(1); y++)
            {
                Vector2Int position = new(x, y);
                GameObject tile = MapTools.MapToTile(position);
                bool isBlocked = false;
                //add impassable test here
                if(tile == null || tile.GetComponent<BattleTileController>()?.unitContents != null)
                {
                    isBlocked = true;
                }
                nodeMap.Add(position, new Node { location = position, reference = tile, blocked = isBlocked });
            }
        }
    }
    public List<GameObject> FindObjectPath(Vector2Int start, Vector2Int end)
    {
        List<Node> path = FindPath(nodeMap[start], nodeMap[end]);
        if(path.Count > 0)
        {
            return path.Select(x => x.reference).ToList();
        }
        return null;
    }
    List<Node> FindPath(Node start, Node end)
    {
        List<Node> openList = new();
        List<Node> closeList = new();

        openList.Add(start);

        while(openList.Count > 0)
        {
            Node current = openList.OrderBy(x => x.F).First();

            openList.Remove(current);
            closeList.Add(current);

            if(current == end)
            {
                //finalize our path.
                return GetFinishedRoute(start,end);
            }

            var neighbors = GetNeighbors(current);

            foreach( Node neighbor in neighbors)
            {
                if(neighbor.blocked || closeList.Contains(neighbor))
                {
                    continue;
                }

                neighbor.G = GetTaxiDistance(start, neighbor);
                neighbor.H = GetTaxiDistance(end, neighbor);

                neighbor.previous = current;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }
        }

        return new List<Node>();
    }

    private List<Node> GetFinishedRoute(Node start, Node end)
    {
        List<Node> finishedList = new();

        Node current = end;

        while(current != start) 
        {
            finishedList.Add(current);
            current = current.previous;
        }

        finishedList.Reverse();
        return finishedList;
    }

    private int GetTaxiDistance(Node start, Node neighbor)
    {
        return Mathf.Abs(start.location.x - neighbor.location.x) + Mathf.Abs(start.location.y - neighbor.location.y);
    }

    private List<Node> GetNeighbors(Node current)
    {
        List<Node> neighbors = new();

        Vector2Int locationCheck = new Vector2Int(current.location.x, current.location.y+1);
        if (nodeMap.ContainsKey(locationCheck)) neighbors.Add(nodeMap[locationCheck]);
        locationCheck = new Vector2Int(current.location.x+1, current.location.y);
        if (nodeMap.ContainsKey(locationCheck)) neighbors.Add(nodeMap[locationCheck]);
        locationCheck = new Vector2Int(current.location.x-1, current.location.y);
        if (nodeMap.ContainsKey(locationCheck)) neighbors.Add(nodeMap[locationCheck]);
        locationCheck = new Vector2Int(current.location.x, current.location.y-1);
        if (nodeMap.ContainsKey(locationCheck)) neighbors.Add(nodeMap[locationCheck]);

        return neighbors;
    }
}

public class Node
{
    public int G;
    public int H;

    public int F { get { return G + H; } }

    //consider switching to vector3
    public Vector2Int location;

    public GameObject reference;

    public bool blocked;

    public Node previous;
}
