using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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

    static readonly float duration = .3f;
    public IEnumerator PlacePlayer(GameObject player)
    {
        int placementIndex = Random.Range(0, playerSpots.Count);
        Vector3 tilePosition = playerSpots[placementIndex].unitPosition;
        Vector3 highPosition = tilePosition;
        highPosition.y += 5f;
        player.transform.position = highPosition;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            Vector3 step = Vector3.Lerp(highPosition, tilePosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            player.transform.position = step;
            yield return null;
        }
        player.transform.position = tilePosition;
        VFXMachine.PlayAtLocation("RoundGust", player.transform.position);
        SoundManager.PlaySound(SoundType.SLIMESTEP);
    }

    public void PlaceEnemy(GameObject unit)
    {
        Vector3 tilePosition;
        if (enemySpots.Count > 0)
        {
            int placementIndex = Random.Range(0, enemySpots.Count);
            tilePosition = enemySpots[placementIndex].unitPosition;
            enemySpots.RemoveAt(placementIndex);
        }
        else
        {
            int placementIndex = Random.Range(0, extraEnemySpots.Count);
            tilePosition = extraEnemySpots[placementIndex].unitPosition;
            extraEnemySpots.RemoveAt(placementIndex);
        }
        GameObject.Instantiate(unit, tilePosition, PhysicsHelper.RandomCardinalRotate());
        
    }

    internal void PlaceBoss(List<int> sequence)
    {
        PlaceEnemy(spawnPool.spawnUnits[sequence[0]]);
        sequence.RemoveAt(0);
    }
}
