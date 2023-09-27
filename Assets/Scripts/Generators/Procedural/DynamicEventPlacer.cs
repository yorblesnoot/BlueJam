using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicEventPlacer
{
    readonly public static int chunkSize = 15;
    Vector2Int currentChunk;
    readonly int totalChance;
    readonly List<int> probabilities;
    RunData runData;

    readonly Dictionary<int, EventType> eventsAndOdds = new()
    {
        { 1, EventType.ITEM}, //item
        { 3, EventType.REMOVE }, //removal
        { 7, EventType.HEAL }, //heal
        { 16, EventType.ENEMY }, //enemy
        { 350, EventType.NONE }, //nothing
    };
    public DynamicEventPlacer(RunData data)
    {
        runData = data;
        totalChance = 0;
        probabilities = new();
        foreach (var item in eventsAndOdds.Keys)
        {
            totalChance += item;
            probabilities.Add(item);
        }
        EventManager.playerAtWorldLocation.AddListener((Vector2Int position) => CheckToPopulateChunks(position));
    }
    public void CheckToPopulateChunks(Vector2Int globalPosition, bool checkCurrent = false)
    {
        Vector2Int chunkLocation = new(globalPosition.x/chunkSize, globalPosition.y/chunkSize);
        if (chunkLocation == currentChunk) return;
        
        PopulateNeighboringChunks(chunkLocation);
        currentChunk = chunkLocation;
        if (checkCurrent) PopulateChunk(chunkLocation);
        else FlagCurrentAsExplored();
    }

    public void FlagCurrentAsExplored()
    {
        runData.exploredChunks[currentChunk.x, currentChunk.y] = true;
    }

    private void PopulateNeighboringChunks(Vector2Int chunkLocation)
    {
        List<Vector2Int> neighbors = chunkLocation.GetSurroundingCoordinates();
        foreach(Vector2Int neighbor in neighbors)
        {
            PopulateChunk(neighbor);
        }
    }

    private void PopulateChunk(Vector2Int chunk)
    {
        bool? hasBeenPopulated = runData.exploredChunks.Safe2DFind(chunk.x, chunk.y);
        if (hasBeenPopulated != false) return;
        runData.score += 5;
        List<Vector2Int> validSpots = GetValidSpots(chunk);
        foreach (Vector2Int validSpot in validSpots)
        {
            EventType output = RandomEvent(eventsAndOdds, totalChance);
            if (output != EventType.NONE) 
                runData.eventMap.Add(validSpot, output);
            else EvaluateSpecialPlacements(validSpot);
        }
        runData.exploredChunks[chunk.x, chunk.y] = true;
    }
    readonly int boatChance = 9;
    readonly int balloonChance = 5;
    private void EvaluateSpecialPlacements(Vector2Int globalPoint)
    {
        PlaceVehicle(globalPoint, EventType.BOAT, new TerrainType[] { TerrainType.WATER, TerrainType.DEEPWATER }, boatChance);
        PlaceVehicle(globalPoint, EventType.BALLOON, new TerrainType[] { TerrainType.MOUNTAIN }, balloonChance);
    }

    private void PlaceVehicle(Vector2Int globalPoint, EventType vehicle, TerrainType[] impassables, int probability)
    {
        if (!impassables.Contains(runData.worldMap[globalPoint.x, globalPoint.y])) return;
        List<Vector2Int> surrounding = globalPoint.GetAdjacentCoordinates();
        foreach(var point in surrounding)
        {
            if (!impassables.Contains(runData.worldMap[point.x, point.y]))
            {
                if (UnityEngine.Random.Range(0, probability) == 0) runData.eventMap.Add(globalPoint, vehicle);
                return;
            }
        }
    }

    private EventType RandomEvent(Dictionary<int, EventType> options, int total)
    {
        int selection = UnityEngine.Random.Range(0, total);
        int currentThreshold = 0;
        foreach(int threshold in options.Keys)
        {
            currentThreshold += threshold;
            if(selection <= currentThreshold)
            {
                selection = threshold;
                break;
            }
        }
        return options[selection];
    }

    List<Vector2Int> GetValidSpots(Vector2Int chunkLocation)
    {
        List<Vector2Int> validSpots = new();
        chunkLocation *= chunkSize;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                //add any extra placement logic here
                Vector2Int spot = new(x, y);
                spot += chunkLocation;
                validSpots.Add(spot);
            }
        }
        return validSpots;
    }

    public void PlaceBoss()
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        direction *= Settings.Balance.BossSpawnDistance;
        Vector2Int intLocation = new(Mathf.RoundToInt(direction.x) + runData.playerWorldX, Mathf.RoundToInt(direction.y) + runData.playerWorldY);
        Vector2Int chunkLocation = intLocation/chunkSize;
        PopulateChunk(chunkLocation);
        runData.eventMap.Remove(intLocation);
        runData.eventMap.Add(intLocation, EventType.BOSS);
    }
}
