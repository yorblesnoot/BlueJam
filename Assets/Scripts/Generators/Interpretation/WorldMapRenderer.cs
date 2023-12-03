using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class WorldMapRenderer : MonoBehaviour
{
    [SerializeField] MapDispenser mapKey;
    public WorldEventRenderer eventRenderer;
    [SerializeField] RunData runData;
    [SerializeField] WorldEventRenderer WorldEventRenderer;

    public static Vector2Int spotlightGlobalOffset;
    public static Vector2Int spotlightLocalOffset;
    
    Vector2Int lastRecordedPlayerLocalPosition;
    readonly int windowRadius = 4;
    static bool[,] windowShape;

    Dictionary<TerrainType, ObjectPool> tilePools;

    Dictionary<GameObject, Animator> animatorCache;

    System.Random rand = new();

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
        tilePools = new();
        foreach (var key in mapKey.Keys)
            tilePools.Add(key, new ObjectPool(mapKey[key]));
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

    [SerializeField] float tileSpeedMinimum = 1;
    [SerializeField] float speedRangeSize = 2;
    void RenderCell(TerrainType tileKey, Vector2Int cellCoords)
    {
        GameObject tile = tilePools[tileKey].InstantiateFromPool(MapTools.MapToVector(cellCoords, 0), PhysicsHelper.RandomCardinalRotate());
        MapTools.gameMap.Add(cellCoords, tile);
        GameObject cellEvent = eventRenderer.RenderCellEvent(cellCoords, spotlightGlobalOffset);
        Animator cachedAnim = GetCachedAnimator(tile);
        cachedAnim.Rebind();
        cachedAnim.Update(0f);
        if (cellEvent != null) cellEvent.transform.SetParent(tile.transform.Find("Model"), true);
        cachedAnim.Play("RiseIn");
        cachedAnim.speed = (float)rand.NextDouble() * speedRangeSize + tileSpeedMinimum;
    }

    [SerializeField] float animationLength = .5f;
    IEnumerator UnrenderCell(Vector2Int coords)
    {
        GameObject toUnrender = MapTools.MapToTile(coords);
        MapTools.gameMap.Remove(coords);
        if (!toUnrender.activeSelf) yield break;
        Animator unrenderAnimator = GetCachedAnimator(toUnrender);
        unrenderAnimator.Play("DropOut");
        float unrenderDelay = animationLength / unrenderAnimator.speed;
        Vector2Int globalCoords = coords + spotlightGlobalOffset;
        if (runData.eventMap.ContainsKey(globalCoords))
            WorldEventRenderer.UnrenderCellEvent(globalCoords, unrenderDelay);
        yield return new WaitForSeconds(unrenderDelay);
        
        if (toUnrender != null)
        {
            tilePools[runData.worldMap[globalCoords.x, globalCoords.y]].ReturnToPool(toUnrender);
        }
    }

    Animator GetCachedAnimator(GameObject spawned)
    {
        animatorCache ??= new();
        if(animatorCache.TryGetValue(spawned, out Animator animator)) return animator;
        Animator output = spawned.GetComponent<Animator>();
        animatorCache.Add(spawned, output);
        return output;
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