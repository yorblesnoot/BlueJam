using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemy : MonoBehaviour
{
    [HideInInspector]public SpawnPool spawnPool;
    bool[,] aggroZone;
    private void Awake()
    {
        PullSpawnPool();
        RegisterAggroZone();
    }
    public virtual void PullSpawnPool()
    {
        //get spawn pool from tile we were spawned on
        GameObject tile = MapTools.VectorToTile(transform.position);
        spawnPool = tile.GetComponent<WorldEventHandler>().tileEnemyPreset;       
    }

    public virtual void RegisterAggroZone()
    {
        aggroZone = MapRulesGenerator.Convert(TileMapShape.CROSS, 1, 0);
        List<GameObject> zone = CellTargeting.ConvertMapRuleToTiles(aggroZone, gameObject.transform.position);
        foreach (GameObject tile in zone)
        {
            tile.GetComponent<WorldEventHandler>().RegisterAggroZone(this);
        }
        //register aggro locations on adjacent cells
    }
}
