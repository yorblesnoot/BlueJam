using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder 
{
    Dictionary<Vector2Int, Node> nodeMap = new();
    
    public Pathfinder(bool occupiedIsUnpathable = true)
    {
        foreach (Vector2Int key in MapTools.gameMap.Keys)
        {
            bool isBlocked = false;
            GameObject tile = MapTools.MapToTile(key);
            if (occupiedIsUnpathable)
            {
                if (tile == null || tile.GetComponent<BattleTileController>()?.unitContents != null)
                {
                    isBlocked = true;
                }
            }
            nodeMap.Add(key, new Node { location = key, reference = MapTools.gameMap[key], blocked = isBlocked });
        }
    }

    public Pathfinder(TerrainType[,] worldMap, Dictionary<Vector2Int, EventType> eventMap, Vector2Int globalOffset)
    {
        foreach (Vector2Int localKey in MapTools.gameMap.Keys)
        {
            Vector2Int globalKey = localKey + globalOffset;
            bool unpathable = false;
            if (RunStarter.unpathable.Contains(worldMap[globalKey.x, globalKey.y]))
            {
                unpathable = true;
                if(WorldPlayerControl.CurrentVehicle != null 
                    && WorldPlayerControl.CurrentVehicle.compatibleTerrains.Contains(worldMap[globalKey.x, globalKey.y]))
                {
                    unpathable = false;
                }
                if (eventMap.TryGetValue(globalKey, out var eventType) && (eventType == EventType.BOAT || eventType == EventType.BALLOON))
                {
                    unpathable = false;
                }
            } 
            nodeMap.Add(localKey, new Node { location = localKey, reference = MapTools.gameMap[localKey], blocked = unpathable });
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

    public List<Vector2Int> FindVectorPath(Vector2Int start, Vector2Int end)
    {
        List<Node> path = FindPath(nodeMap[start], nodeMap[end]);
        if (path.Count > 0)
        {
            return path.Select(x => x.location).ToList();
        }
        return null;
    }

    public int GetPathLength(Vector2Int start, Vector2Int end)
    {
        List<Node> path = FindPath(nodeMap[start], nodeMap[end]);
        return path.Count;
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

        Vector2Int locationCheck = new(current.location.x, current.location.y+1);
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

class Node
{
    public int G;
    public int H;

    public int P;
    public int F { get { return G + H + P; } }

    //consider switching to vector3
    public Vector2Int location;

    public GameObject reference;

    public bool blocked;

    public Node previous;
}
