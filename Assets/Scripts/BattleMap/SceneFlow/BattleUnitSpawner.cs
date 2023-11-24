using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleUnitSpawner
{

    Dictionary<Vector2Int, GameObject> battleMap;
    List<BattleTileController> otherSpots = new();
    SpawnPool spawnPool;
    MasterEnemyPool masterEnemyPool;

    Vector3 playerSpawn;
    public BattleUnitSpawner(SpawnPool pool, Dictionary<Vector2Int, GameObject> map, MasterEnemyPool master)
    {
        spawnPool = pool;
        battleMap = map;
        masterEnemyPool = master;
        //loop through every battle map spot and add it to a list of valid cell placements
        GetValidSpots();
        playerSpawn = DesignatePlayerSpawnLocation();
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

    private Vector3 DesignatePlayerSpawnLocation()
    {
        int placementIndex = Random.Range(0, otherSpots.Count);
        Vector3 tilePosition = otherSpots[placementIndex].unitPosition;
        otherSpots.RemoveAt(placementIndex);
        return tilePosition;
    }

    void GetValidSpots()
    {
        foreach (Vector2Int key in battleMap.Keys)
        {
            GameObject tile = battleMap[key];
            if (tile != null)
            {
                BattleTileController battleTileController = tile.GetComponent<BattleTileController>();
                otherSpots.Add(battleTileController);
            }
        }
    }

    static readonly float duration = .3f;
    public IEnumerator PlacePlayer(GameObject player)
    {
        Vector3 highPosition = playerSpawn;
        highPosition.y += 5f;
        player.transform.position = highPosition;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            Vector3 step = Vector3.Lerp(highPosition, playerSpawn, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            player.transform.position = step;
            yield return null;
        }
        player.transform.position = playerSpawn;
        VFXMachine.PlayAtLocation("RoundGust", player.transform.position);
        SoundManager.PlaySound(SoundType.SLIMESTEP);
    }

    readonly int deviation = 10;
    public void PlaceEnemy(GameObject unit)
    {
        Vector3 tilePosition;
        GameObject spawned = GameObject.Instantiate(unit, Vector3.zero, PhysicsHelper.RandomCardinalRotate());
        UnitAI unitAI = spawned.GetComponent<UnitAI>();

        otherSpots = otherSpots.OrderBy(x => GetPositionScore(unitAI, x)).ToList();
        int placementIndex = Random.Range(0, deviation);
        tilePosition = otherSpots[placementIndex].unitPosition;
        otherSpots.RemoveAt(placementIndex);
        spawned.transform.position = tilePosition;
    }

    readonly int graceDistance = 1;
    float GetPositionScore(UnitAI unit, BattleTileController tile)
    {
        return Mathf.Abs((playerSpawn - tile.unitPosition).magnitude - (unit.personality.proximityHostile + graceDistance));
    }

    internal void PlaceBoss(List<int> sequence)
    {
        PlaceEnemy(spawnPool.spawnUnits[sequence[0]]);
        sequence.RemoveAt(0);
    }
}
