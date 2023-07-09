using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapRenderer : MonoBehaviour
{
    [SerializeField] MapKey mapKey;
    [SerializeField] WorldEventRenderer eventRenderer;
    [SerializeField] RunData runData;
    public static Vector2Int spotlightGlobalOffset;
    public static Vector2Int spotlightLocalOffset;
    bool[,] windowShape;
    Vector2Int lastRecordedPlayerLocalPosition;

    public void RenderInitialWorldWindow(string[,] worldMap, int windowRadius)
    {
        MapTools.gameMap = new();
        windowShape = GenerateWindowShape(windowRadius);

        Vector2Int windowCenter = new(runData.playerWorldX, runData.playerWorldY);
        RenderFromFullWindow(windowShape, worldMap, windowCenter);

        EventManager.playerAtWorldLocation.AddListener(ShiftWindow);
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

    public void RenderFromFullWindow(bool[,] localMap, string[,] globalMap, Vector2Int globalCenter)
    {
        //project a boolean grid onto a larger string grid, then render from the string grid based on the overlap
        int localSize = localMap.GetLength(0);
        spotlightLocalOffset = new(localSize / 2, localSize / 2);
        spotlightGlobalOffset = globalCenter - spotlightLocalOffset;
        lastRecordedPlayerLocalPosition = spotlightLocalOffset;
        mapKey.Initialize();
        for (int x = 0; x < localSize; x++)
        {
            for (int y = 0; y < localSize; y++)
            {  
                if (localMap[x, y] == true)
                {
                    Vector2Int localPosition = new(x, y);
                    Vector2Int globalPosition = localPosition + spotlightGlobalOffset;
                    string tileKey = globalMap[globalPosition.x, globalPosition.y];
                    RenderCell(tileKey, localPosition);
                }
            }
        }
    }

    void RenderCell(string tileKey, Vector2Int cellCoords)
    {
        //check if we have previously rendered the cell, if yes re-enable it otherwise create it and associated events
        if (MapTools.gameMap.ContainsKey(cellCoords))
        {
            MapTools.gameMap[cellCoords].SetActive(true);
            StartCoroutine(StaggeredMoveIn(MapTools.gameMap[cellCoords], -5f, 0f));
        }
        else
        {
            GameObject tile = Instantiate(mapKey.hashKey[tileKey], MapTools.MapToVector(cellCoords, 0), Quaternion.identity);
            MapTools.gameMap.Add(cellCoords, tile);
            GameObject renderedEvent = eventRenderer.RenderCellEvent(cellCoords, spotlightGlobalOffset);
            if(renderedEvent != null) renderedEvent.transform.SetParent(tile.transform);
            StartCoroutine(StaggeredMoveIn(tile, -5f, 0f));
        }
    }

    IEnumerator StaggeredMoveIn(GameObject riser, float startElevation, float endElevation)
    {
        //figure out where to put the object at the start
        Vector3 startPosition = riser.transform.position;
        startPosition.y = startElevation;
        //find the target location
        Vector3 finalPosition = riser.transform.position;
        finalPosition.y = endElevation;

        riser.transform.position = startPosition;

        //get the distance to move
        float distance = Mathf.Abs(endElevation - startElevation);
        //use a random final travel time and figure out how many steps to take to arrive at the final destination
        float travelTime = Random.Range(.2f, .4f);
        float stepDelay = .01f;
        int steps = Mathf.RoundToInt(travelTime / stepDelay);
        float stepSize = distance / steps;
        for(int i = 0; i < steps; i++)
        {
            riser.transform.position = Vector3.MoveTowards(riser.transform.position, finalPosition, stepSize);
            yield return new WaitForSeconds(stepDelay);
        }
    }

    IEnumerator UnrenderCell(Vector2Int coords)
    {
        GameObject toUnrender = MapTools.MapToTile(coords);
        if (toUnrender != null)
        {
            yield return StartCoroutine(StaggeredMoveIn(toUnrender, 0f, -5f));
            toUnrender.SetActive(false);
        }
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