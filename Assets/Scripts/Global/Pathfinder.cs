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
        Dictionary<Vector2Int, GameObject> pathingMap = new(MapTools.tileMap.forward);
        if(RiftMaker.riftedCells != null && RiftMaker.riftedCells.Count > 0)
        {
            foreach (var cell in RiftMaker.riftedCells)
            {
                pathingMap.Remove(cell);
            }
        }
        
        foreach (Vector2Int key in pathingMap.Keys)
        {
            bool isBlocked = false;
            GameObject tile = MapTools.TileAtMapPosition(key);
            if (occupiedIsUnpathable)
            {
                if (tile == null || tile.OccupyingUnit() != null)
                {
                    isBlocked = true;
                }
            }
            nodeMap.Add(key, new Node { location = key, reference = MapTools.tileMap[key], blocked = isBlocked });
        }
    }

    public Pathfinder(TerrainType[,] worldMap, Dictionary<Vector2Int, EventType> eventMap, Vector2Int globalOffset, Vector2Int selectedCellCoords)
    {
        Vector2Int selectCoords = selectedCellCoords + globalOffset;
        foreach (Vector2Int localKey in MapTools.tileMap.forward.Keys)
        {
            Vector2Int globalKey = localKey + globalOffset;
            bool unpathable = false;
            int penalty = 0;

            bool inVehicle = WorldPlayerControl.CurrentVehicle != null;
            bool mouseOverCompatible = inVehicle && WorldPlayerControl.CurrentVehicle.compatibleTerrains.Contains(worldMap[selectCoords.x, selectCoords.y]);
            bool nodeIsCompatibleWithVehicle = inVehicle && WorldPlayerControl.CurrentVehicle.compatibleTerrains.Contains(worldMap[globalKey.x, globalKey.y]);
            bool nodeIsBlocking = RunStarter.unpathable.Contains(worldMap[globalKey.x, globalKey.y]);
            bool nodeContainsVehicle = eventMap.TryGetValue(globalKey, out var eventType) && (eventType == EventType.BOAT || eventType == EventType.BALLOON);

            //if im not in the right vehicle, blocking terrain is unpathable and non blocking is pathable
            //if i am in a vehicle and im mousing over compatible blocking terrain, non-blocking terrain is unpathable
            //if i am in a vehicle and not mousing over blocking terrain, compatible terrain and non-blocking terrain is pathable
            if (!inVehicle)
            {
                if (nodeIsBlocking && !nodeContainsVehicle)
                {
                    unpathable = true;
                }
            }
            else if (inVehicle)
            {
                if (!nodeIsCompatibleWithVehicle)
                {
                    if (mouseOverCompatible && !nodeIsBlocking)
                    {
                        unpathable = true;
                    }
                    if (!mouseOverCompatible && nodeIsBlocking)
                    {
                        unpathable = true;
                    }
                }
            }

            nodeMap.Add(localKey, new Node { location = localKey, reference = MapTools.tileMap[localKey], blocked = unpathable, P = penalty });
        }
    }
    public List<GameObject> FindObjectPath(Vector2Int start, Vector2Int end)
    {
        List<Node> path = FindPath(start, end);
        if (path == null || path.Count == 0) return null;
        return path.Select(x => x.reference).ToList();
    }

    public List<Vector2Int> FindVectorPath(Vector2Int start, Vector2Int end)
    {
        List<Node> path = FindPath(start, end);
        if (path == null || path.Count == 0) return null;
        return path.Select(x => x.location).ToList();
    }

    public int GetPathLength(Vector2Int start, Vector2Int end)
    {
        List<Node> path = FindPath(start, end);
        if (path != null) return path.Count;
        else return -1;
    }
    List<Node> FindPath(Vector2Int startCoords, Vector2Int endCoords)
    {
        if(!nodeMap.TryGetValue(startCoords, out Node start) || !nodeMap.TryGetValue(endCoords, out Node end)) return null;
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
