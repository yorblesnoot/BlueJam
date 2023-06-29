using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralEventPlacer
{
    //make this spawn all kinds of world objects, not just enemies ~~~~~~~~~~~~~~~~~~~~~~~~

    string[,] map;
    List<int[]> enemyLocations;
    string[,] worldEvents;

    List<int[]> validSpots;
    int enemyCap;
    int keyCap = 3;
    int removeCap = 4;
    int itemCap = 3;
    int healCap = 3;

    RunData runData;
    public ProceduralEventPlacer(RunData data)
    {
        runData = data;
        //number of events to place
        map = data.worldMap;
        worldEvents = new string[map.GetLength(0),map.GetLength(1)];
        enemyCap = map.GetLength(0);
        enemyLocations = new();
        //loop through every battle map spot and add it to a list of valid cell placements
        GetValidSpots();
    }

    void GetValidSpots()
    {
        validSpots = new List<int[]>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                //add any extra placement logic here
                validSpots.Add(new int[] { x, y });
            }
        }
        validSpots.RemoveCoordinates(new int[] { runData.playerWorldX, runData.playerWorldY });
        validSpots.RemoveCoordinates(new int[] { runData.bossWorldX, runData.bossWorldY });
    }
    public List<int[]> PlaceEnemies()
    {
        //switch this to putting enemies on the event map ~~~~~~~~~~~~~~~~~~~~~~
        for(int i = 0; i < enemyCap; i++)
        {
            int placementIndex = Random.Range(0, validSpots.Count - 1);
            int[] coords = validSpots[placementIndex];
            enemyLocations.Add(coords);
            validSpots.RemoveAt(placementIndex);
        }
        return enemyLocations;
    }

    public string[,] PlaceWorldEvents()
    {
        PlaceToCap("k", keyCap);
        PlaceToCap("r", removeCap);
        PlaceToCap("i", itemCap);
        PlaceToCap("h", healCap);
        return worldEvents;
    }

    public void PlaceToCap(string thing, int cap)
    {
        for (int i = 0; i < cap; i++)
        {
            int placementIndex = Random.Range(0, validSpots.Count - 1);
            int[] coords = validSpots[placementIndex];
            worldEvents[coords[0], coords[1]] = thing;
            validSpots.RemoveAt(placementIndex);
        }
    }
}
