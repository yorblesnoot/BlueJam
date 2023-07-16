using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleUnitSpawner
{

    Dictionary<Vector2Int, GameObject> battleMap;
    List<BattleTileController> playerSpots;
    List<BattleTileController> enemySpots;
    List<BattleTileController> extraEnemySpots;
    SpawnPool spawnPool;
    MasterEnemyPool masterEnemyPool;
    public BattleUnitSpawner(SpawnPool pool, Dictionary<Vector2Int, GameObject> map, MasterEnemyPool master)
    {
        spawnPool = pool;
        battleMap = map;
        masterEnemyPool = master;
        playerSpots = new();
        enemySpots = new();
        extraEnemySpots = new();
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
                enemySpots.Add(battleTileController);
            else if(battleTileController.spawns == BattleTileController.SpawnPermission.PLAYER)
                playerSpots.Add(battleTileController);  
            else extraEnemySpots.Add(battleTileController);
        }
    }

    public void PlaceEnemies(int budget)
    {
        masterEnemyPool.Initialize();
        if (spawnPool.spawnUnits.Count > 0)
        {
            while (budget > 0)
            {
                int enemyIndex = Random.Range(0, spawnPool.spawnUnits.Count);
                int enemyWeight = masterEnemyPool.masterPool[spawnPool.spawnUnits[enemyIndex]];
                if (enemyWeight <= budget)
                {
                    budget -= enemyWeight;
                    PlaceEnemy(spawnPool.spawnUnits[enemyIndex]);
                }
            }
        }
        foreach (GameObject spawn in spawnPool.staticSpawns)
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
        Vector3 tilePosition;
        if (enemySpots.Count > 0)
        {
            int placementIndex = Random.Range(0, enemySpots.Count - 1);
            tilePosition = enemySpots[placementIndex].unitPosition;
            enemySpots.RemoveAt(placementIndex);
        }
        else
        {
            int placementIndex = Random.Range(0, extraEnemySpots.Count - 1);
            tilePosition = extraEnemySpots[placementIndex].unitPosition;
            extraEnemySpots.RemoveAt(placementIndex);
        }
        GameObject.Instantiate(unit, tilePosition, Quaternion.identity);
        
    }
}
