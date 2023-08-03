using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEventPlacer
{
    readonly public static int chunkSize = 10;
    Vector2Int currentChunk;
    readonly int totalChance;
    readonly List<int> probabilities;
    RunData runData;

    readonly Dictionary<int, string> eventsAndOdds = new()
    {
        { 1, "i"}, //item
        { 3, "r" }, //removal
        { 6, "h" }, //heal
        { 17, "e" }, //enemy
        { 300, "" }, //nothing
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
        EventManager.playerAtWorldLocation.AddListener(CheckToPopulateChunks);
    }
    public void CheckToPopulateChunks(Vector2Int globalPosition)
    {
        Vector2Int chunkLocation = new(globalPosition.x/chunkSize, globalPosition.y/chunkSize);
        if(chunkLocation != currentChunk)
        {
            PopulateNeighboringChunks(chunkLocation);
            currentChunk = chunkLocation;
        }
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
            string output = RandomEvent(eventsAndOdds, totalChance);
            if (!string.IsNullOrEmpty(output)) 
                runData.eventMap.Add(validSpot, output);
        }
        runData.exploredChunks[chunk.x, chunk.y] = true;
    }

    private string RandomEvent(Dictionary<int, string> options, int total)
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
        runData.eventMap.Add(intLocation,"b");
    }
}
