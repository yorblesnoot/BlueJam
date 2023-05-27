using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterBuilder
{
    public List<GameObject> spawnUnits;
    public List<int> spawnWeights;

    string[,] battleMap;
    List<int[]> validSpots;
    public EncounterBuilder(List<GameObject> units, List<int> weights, string[,] map)
    {
        spawnUnits = units;
        spawnWeights = weights;
        battleMap = map;

        validSpots = new List<int[]>();
        //loop through ever battle map spot and add it to a list of valid cell placements
        for(int x = 0; x < battleMap.GetLength(0); x++)
        {
            for(int y = 0;  y < battleMap.GetLength(1); y++) 
            {
                if (battleMap[x,y] != "x")
                {
                    validSpots.Add(new int[] {x,y});
                }
            }
        }
    }

    public void PlaceEnemies(int budget)
    {
        while(budget > 0)
        {
            int enemyIndex = Random.Range(0, spawnUnits.Count);
            int enemyWeight = spawnWeights[enemyIndex];
            if (enemyWeight <= budget)
            {
                budget -= enemyWeight;
                PlaceUnit(spawnUnits[enemyIndex]);
            }
        }
    }

    public void PlaceUnit(GameObject unit)
    {
        int placementIndex = Random.Range(0, validSpots.Count - 1);
        int[] coords = validSpots[placementIndex];
        GameObject tile = GridTools.VectorToTile(GridTools.MapToVector(coords[0], coords[1], 0));
        Vector3 tilePosition = tile.GetComponent<BattleTileController>().unitPosition;
        if (unit.tag == "Player")
        {
            unit.transform.position = tilePosition;
        }
        else
        {
            GameObject.Instantiate(unit, tilePosition, Quaternion.identity);
        }
        validSpots.RemoveAt(placementIndex);
    }
}
