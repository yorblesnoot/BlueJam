using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralEventPlacer
{
    //make this spawn all kinds of world objects, not just enemies ~~~~~~~~~~~~~~~~~~~~~~~~

    string[,] map;
    List<int[]> enemyLocations;

    List<int[]> validSpots;
    int eventCap;

    RunData runData;
    public ProceduralEventPlacer(RunData data)
    {
        runData = data;
        //number of events to place
        map = data.worldMap;
        eventCap = 2 * map.GetLength(0);
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
        validSpots.Remove(new int[] { runData.worldX, runData.worldY });
    }
    public List<int[]> PlaceEnemies()
    {
        //switch this to putting enemies on the event map ~~~~~~~~~~~~~~~~~~~~~~
        for(int i = 0; i < eventCap; i++)
        {
            int placementIndex = Random.Range(0, validSpots.Count - 1);
            int[] coords = validSpots[placementIndex];
            enemyLocations.Add(coords);
            validSpots.RemoveAt(placementIndex);
        }
        return enemyLocations;
    }

    public string RandomizeEvent()
    {
        //make it so we pick randomly from items and enemies~~~~~~~~~
        string eventType = "e";
        return eventType;
    }
}
