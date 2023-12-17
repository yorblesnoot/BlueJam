using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleUnitSpawner
{

    Dictionary<Vector2Int, GameObject> battleMap;
    List<BattleTileController> validSpots = new();
    SpawnPool spawnPool;
    MasterEnemyPool masterEnemyPool;

    List<GameObject> currentlySpawned;

    BattleTileController playerSpawnTile;
    public BattleUnitSpawner(SpawnPool pool, MasterEnemyPool master)
    {
        currentlySpawned = new();
        spawnPool = pool;
        battleMap = MapTools.tileMap.forward;
        masterEnemyPool = master;
        //loop through every battle map spot and add it to a list of valid cell placements
        GetValidSpots();
        DesignatePlayerSpawnLocation();
    }

    public void SmartSpawn(int budget)
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

    private void DesignatePlayerSpawnLocation()
    {
        int placementIndex = Random.Range(0, validSpots.Count);
        playerSpawnTile = validSpots[placementIndex];
        validSpots.RemoveAt(placementIndex);
    }

    void GetValidSpots()
    {
        foreach (Vector2Int key in battleMap.Keys)
        {
            GameObject tile = battleMap[key];
            if (tile != null)
            {
                BattleTileController battleTileController = tile.GetComponent<BattleTileController>();
                if (battleTileController.IsRift) continue;
                validSpots.Add(battleTileController);
            }
        }
    }

    static readonly float duration = .3f;
    public IEnumerator PlacePlayer(GameObject player)
    {
        Vector3 highPosition = playerSpawnTile.unitPosition;
        highPosition.y += 5f;
        player.transform.position = highPosition;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            Vector3 step = Vector3.Lerp(highPosition, playerSpawnTile.unitPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            player.transform.position = step;
            yield return null;
        }
        player.transform.position = playerSpawnTile.unitPosition;
        MapTools.ReportPositionChange(player.GetComponent<PlayerUnit>(), playerSpawnTile);
        VFXMachine.PlayAtLocation("RoundGust", player.transform.position);
        SoundManager.PlaySound(SoundType.SLIMESTEP);
    }

    readonly int deviation = 5;
    public void PlaceEnemy(GameObject unit)
    {
        Vector3 tilePosition;
        GameObject spawned = GameObject.Instantiate(unit, Vector3.zero, PhysicsHelper.RandomCardinalRotate());
        
        UnitAI unitAI = spawned.GetComponent<UnitAI>();
        validSpots = validSpots.OrderBy(x => GetPositionScore(unitAI, x)).ToList();

        currentlySpawned.Add(spawned);

        int placementIndex = Random.Range(0, deviation);
        tilePosition = validSpots[placementIndex].unitPosition;
        MapTools.ReportPositionChange(spawned.GetComponent<BattleUnit>(), validSpots[placementIndex]);
        validSpots.RemoveAt(placementIndex);
        spawned.transform.position = tilePosition;
    }

    readonly int graceDistance = 1;
    readonly float allyPenalty = 3f;
    float GetPositionScore(UnitAI unit, BattleTileController tile)
    {
        float playerScore = Mathf.Abs((playerSpawnTile.unitPosition - tile.unitPosition).magnitude - (unit.personality.proximityHostile + graceDistance));
        float allyScore = 0;
        if(currentlySpawned.Count > 0)
        {
            foreach(var spawn in currentlySpawned)
            {
                allyScore += Mathf.Clamp(allyPenalty - (spawn.transform.position - tile.unitPosition).magnitude, 0, allyPenalty);
            }
        }
        return playerScore + allyScore;
    }

    internal void PlaceBoss(List<int> sequence)
    {
        PlaceEnemy(spawnPool.spawnUnits[sequence[0]]);
        sequence.RemoveAt(0);
    }
}
