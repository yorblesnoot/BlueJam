using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleUnitSpawner
{
    public List<GameObject> staticSpawns;
    public List<GameObject> spawnUnits;
    public List<int> spawnWeights;

    Dictionary<Vector2Int, GameObject> battleMap;
    List<BattleTileController> playerSpots;
    List<BattleTileController> enemySpots;
    public BattleUnitSpawner(List<GameObject> statics, List<GameObject> units, List<int> weights, Dictionary<Vector2Int, GameObject> map)
    {
        staticSpawns = statics;
        spawnUnits = units;
        spawnWeights = weights;
        battleMap = map;

        playerSpots = new();
        enemySpots = new();
        //loop through every battle map spot and add it to a list of valid cell placements
        foreach(Vector2Int key in battleMap.Keys) CheckValidSpot(key);
    }



    void CheckValidSpot(Vector2Int position)
    {
        GameObject tile = battleMap[position];
        if(tile != null)
        {
            BattleTileController battleTileController = tile.GetComponent<BattleTileController>();
            if(battleTileController.spawns == BattleTileController.SpawnPermission.ENEMY)
            {
                enemySpots.Add(battleTileController);
            }
            else if(battleTileController.spawns == BattleTileController.SpawnPermission.PLAYER)
            {
                playerSpots.Add(battleTileController);  
            }
        }
    }

    public void PlaceEnemies(int budget)
    {
        if (spawnUnits.Count > 0)
        {
            while (budget > 0)
            {
                int enemyIndex = Random.Range(0, spawnUnits.Count);
                int enemyWeight = spawnWeights[enemyIndex];
                if (enemyWeight <= budget)
                {
                    budget -= enemyWeight;
                    PlaceEnemy(spawnUnits[enemyIndex]);
                }
            }
        }
        foreach (GameObject spawn in staticSpawns)
        {
            PlaceEnemy(spawn);
        }
    }

    public void PlacePlayer(GameObject player)
    {
        int placementIndex = Random.Range(0, playerSpots.Count - 1);
        Vector3 tilePosition = playerSpots[placementIndex].unitPosition;
        player.transform.position = tilePosition;
    }

    public void PlaceEnemy(GameObject unit)
    {
        int placementIndex = Random.Range(0, enemySpots.Count - 1);
        Vector3 tilePosition = enemySpots[placementIndex].unitPosition;
        GameObject.Instantiate(unit, tilePosition, Quaternion.identity);
        enemySpots.RemoveAt(placementIndex);
    }
}
