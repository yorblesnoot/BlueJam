using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicEventPlacer
{
    readonly public static int chunkSize = 15;
    Vector2Int currentChunk;
    
    RunData runData;
    EventSpawnRates eventRates;

    public DynamicEventPlacer(RunData data, EventSpawnRates eventRates)
    {
        eventRates.Initialize();
        runData = data;
        this.eventRates = eventRates;
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
            EventType output = eventRates.RandomEvent();
            if (output != EventType.NONE) 
                runData.eventMap.Add(validSpot, output);
            else EvaluateSpecialPlacements(validSpot);
        }
        runData.exploredChunks[chunk.x, chunk.y] = true;
    }
    private void EvaluateSpecialPlacements(Vector2Int globalPoint)
    {
        foreach(var vehicle in eventRates.vehicles)
        {
            PlaceVehicle(globalPoint, vehicle.type, vehicle.compatibleTerrain, vehicle.spawnChance);
        }
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
        direction *= Settings.Balance[BalanceParameter.BossDistance];
        Vector2Int intLocation = new(Mathf.RoundToInt(direction.x) + runData.playerWorldX, Mathf.RoundToInt(direction.y) + runData.playerWorldY);
        Vector2Int chunkLocation = intLocation/chunkSize;
        PopulateChunk(chunkLocation);
        runData.eventMap.Remove(intLocation);
        runData.eventMap.Add(intLocation, EventType.BOSS);

        TerrainType bossTile = runData.worldMap[intLocation.x, intLocation.y];
        if (!RunStarter.unpathable.Contains(bossTile)) return;
        
        Vector2Int vehicleLocation = runData.worldMap.LineSearch(RunStarter.unpathable, intLocation, GridHelper.RandomCardinalDirection());
        if (vehicleLocation == intLocation)
        {
            runData.worldMap[intLocation.x, intLocation.y] = TerrainType.DESERT;
            return;
        }
        runData.eventMap.Remove(vehicleLocation);
        if (bossTile == TerrainType.WATER || bossTile == TerrainType.DEEPWATER) runData.eventMap.Add(vehicleLocation, EventType.BOAT);
        else if (bossTile == TerrainType.MOUNTAIN) runData.eventMap.Add(vehicleLocation, EventType.BALLOON);
    }
}
