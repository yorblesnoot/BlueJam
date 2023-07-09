using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEventPlacer
{
    readonly public static int chunkSize = 10;
    Vector2Int currentChunk;
    readonly string[,] worldMap;
    readonly int totalChance;
    readonly List<int> probabilities;
    RunData runData;

    readonly Dictionary<int, string> eventsAndOdds = new()
    {
        { 2, "i"}, //item
        { 4, "r" }, //removal
        { 6, "h" }, //heal
        { 12, "e" }, //enemy
        { 200, "" }, //nothing
    };
    public DynamicEventPlacer(RunData data)
    {
        runData = data;
        worldMap = data.worldMap;
        totalChance = 0;
        probabilities = new();
        foreach (var item in eventsAndOdds.Keys)
        {
            totalChance += item;
            probabilities.Add(item);
        }
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
            bool? chunk = runData.exploredChunks.Safe2DFind(neighbor.x, neighbor.y);
            if (chunk != false) continue;
            PopulateChunk(neighbor);
            runData.exploredChunks[neighbor.x, neighbor.y] = true;
        }
    }

    private void PopulateChunk(Vector2Int chunk)
    {
        List<Vector2Int> validSpots = GetValidSpots(chunk);
        foreach (Vector2Int validSpot in validSpots)
        {
            string output = RandomEvent(eventsAndOdds, totalChance);
            if (!string.IsNullOrEmpty(output))
            {
                //Debug.Log($"adding event {output} at {validSpot} in chunk {chunk}");
                runData.eventMap.Add(validSpot, output);
                
            }
        }
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
                validSpots.Add(new Vector2Int(chunkLocation.x + x, chunkLocation.y + y));
            }
        }
        return validSpots;
    }
}
