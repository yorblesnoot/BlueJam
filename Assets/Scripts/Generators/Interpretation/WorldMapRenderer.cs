using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldMapRenderer : MonoBehaviour
{
    [SerializeField] MapKey mapKey;
    [SerializeField] WorldEventRenderer eventRenderer;
    [SerializeField] RunData runData;
    public static Vector2Int spotlightGlobalOffset;
    public static Vector2Int spotlightLocalOffset;
    
    Vector2Int lastRecordedPlayerLocalPosition;
    readonly int windowRadius = 4;
    static bool[,] windowShape;

    Dictionary<TerrainType, ObjectPool> tilePools;

    public void Initialize()
    {
        MapTools.gameMap = new();
        windowShape = GenerateWindowShape(windowRadius);
        SetKeyParameters(windowShape);
        EventManager.playerAtWorldLocation.AddListener(ShiftWindow);
    }

    public void SetKeyParameters(bool[,] windowShape)
    {
        int windowSize = windowShape.GetLength(0);
        spotlightLocalOffset = new(windowSize / 2, windowSize / 2);
        lastRecordedPlayerLocalPosition = spotlightLocalOffset;
        spotlightGlobalOffset = new Vector2Int(runData.playerWorldX, runData.playerWorldY) - spotlightLocalOffset;
    }

    public bool[,] GenerateWindowShape(int windowRadius)
    {
        //create a circular shape in a boolean array
        int mapSize = windowRadius * 2 + 1;
        Vector2Int centerPoint = new(windowRadius, windowRadius);
        bool[,] circleContents = new bool[mapSize,mapSize];
        for(int x = 0; x < mapSize; x++)
        {
            for(int y = 0; y < mapSize; y++)
            {
                Vector2Int point = new(x, y);
                if (Vector2Int.Distance(centerPoint, point) <= windowRadius) circleContents[x, y] = true;
                else circleContents[x, y] = false;
            }
        }
        return circleContents;
    }

    public static TerrainType[,] GetLocalMapFromWindow(TerrainType[,] globalMap)
    {
        //project a boolean grid onto a larger string grid
        int localSize = windowShape.GetLength(0);
        TerrainType[,] localMap = new TerrainType[localSize,localSize];
        for (int x = 0; x < localSize; x++)
        {
            for (int y = 0; y < localSize; y++)
            {
                if (windowShape[x, y] == true)
                {
                    Vector2Int localPosition = new(x, y);
                    Vector2Int globalPosition = localPosition + spotlightGlobalOffset;
                    localMap[x,y] = globalMap[globalPosition.x, globalPosition.y];
                }
            }
        }
        return localMap;
    }

    public void RenderFullWindow(TerrainType[,] globalMap)
    {
        TerrainType[,] localMap = GetLocalMapFromWindow(globalMap);
        //project a boolean grid onto a larger string grid, then render from the string grid based on the overlap
        int localSize = localMap.GetLength(0);
        mapKey.Initialize();
        tilePools = new();
        foreach (var key in mapKey.hashKey.Keys)
            tilePools.Add(key, new ObjectPool(mapKey.hashKey[key]));
        for (int x = 0; x < localSize; x++)
        {
            for (int y = 0; y < localSize; y++)
            {
                TerrainType tileKey = localMap[x, y];
                if (tileKey == TerrainType.EMPTY) continue;
                RenderCell(tileKey, new Vector2Int(x,y));
            }
        }
    }

    void RenderCell(TerrainType tileKey, Vector2Int cellCoords)
    {
        GameObject tile = tilePools[tileKey].InstantiateFromPool(MapTools.MapToVector(cellCoords, 0), PhysicsHelper.RandomCardinalRotate());
        MapTools.gameMap.Add(cellCoords, tile);
        GameObject cellEvent = eventRenderer.RenderCellEvent(cellCoords, spotlightGlobalOffset);
        if(cellEvent != null) cellEvent.transform.SetParent(tile.transform, true);
        StartCoroutine(StaggeredMoveIn(tile, -5f, 0f));
    }

    IEnumerator UnrenderCell(Vector2Int coords)
    {
        GameObject toUnrender = MapTools.MapToTile(coords);
        MapTools.gameMap.Remove(coords);
        if (!toUnrender.activeSelf) yield break;
        yield return StartCoroutine(StaggeredMoveIn(toUnrender, 0f, -5f));
        Vector2Int globalCoords = coords + spotlightGlobalOffset;
        if (runData.eventMap.ContainsKey(globalCoords))
            eventRenderer.UnrenderCellEvent(globalCoords);
        if (toUnrender != null)
        {
            tilePools[runData.worldMap[globalCoords.x, globalCoords.y]].ReturnToPool(toUnrender);
        }
    }

    IEnumerator StaggeredMoveIn(GameObject riser, float startElevation, float endElevation)
    {
        if (riser == null) yield break;
        //figure out where to put the object at the start
        Vector3 startPosition = riser.transform.position;
        startPosition.y = startElevation;
        //find the target location
        Vector3 finalPosition = riser.transform.position;
        finalPosition.y = endElevation;

        riser.transform.position = startPosition;

        //use a random final travel time to get a staggered effect
        float travelTime = Random.Range(.2f, .35f);
        yield return StartCoroutine(riser.LerpTo(finalPosition, travelTime));
    }

    

    public void ShiftWindow(Vector2Int playerGlobalPosition)
    {
        Vector2Int playerLocalPosition = playerGlobalPosition - spotlightGlobalOffset;
        Vector2Int displacement = playerLocalPosition - lastRecordedPlayerLocalPosition;
        lastRecordedPlayerLocalPosition = playerLocalPosition;
        int size = windowShape.GetLength(0);
        //get the negative and positive difference between the new window and the old window
        List<Vector2Int> positiveDifference = new();
        List<Vector2Int> negativeDifference = new();
        for (int x = -1; x < size+1; x++)
        {
            for (int y = -1; y < size+1; y++)
            {
                Vector2Int newCoords = new(x, y);
                Vector2Int oldCoords = newCoords+displacement;

                if (windowShape.Safe2DFind(newCoords.x,newCoords.y) == true && windowShape.Safe2DFind(oldCoords.x, oldCoords.y) != true)
                    positiveDifference.Add(newCoords);
                else if (windowShape.Safe2DFind(newCoords.x, newCoords.y) != true && windowShape.Safe2DFind(oldCoords.x, oldCoords.y) == true)
                    negativeDifference.Add(newCoords);
            }
        }

        //render the positive difference and unrender the negative difference
        foreach(var spotlightShapeDifference in positiveDifference)
        {
            Vector2Int newPosition = spotlightShapeDifference + playerLocalPosition - spotlightLocalOffset;
            Vector2Int worldPosition = newPosition + spotlightGlobalOffset;
            RenderCell(runData.worldMap[worldPosition.x, worldPosition.y], newPosition);
        }

        foreach (var spotlightShapeDifference in negativeDifference)
        {
            Vector2Int newPosition = spotlightShapeDifference + playerLocalPosition - spotlightLocalOffset;
            StartCoroutine(UnrenderCell(newPosition));
        }
    }

}