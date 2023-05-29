using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemy : MonoBehaviour
{
    [HideInInspector]public SpawnPool spawnPool;
    string[,] aggroZone;
    public void PullSpawnPool()
    {
        //get spawn pool from tile we were spawned on
        GameObject tile = GridTools.VectorToTile(transform.position);
        spawnPool = tile.GetComponent<WorldEventHandler>().tileEnemyPreset;       
    }

    public void RegisterAggroZone()
    {
        aggroZone = MapRulesGenerator.Convert(TileMapShape.CROSS, 1, 0);
        List<GameObject> zone = ZoneTargeter.ConvertMapRuleToTiles(aggroZone, gameObject.transform.position);
        foreach (GameObject tile in zone)
        {
            tile.GetComponent<WorldEventHandler>().ActivateAggroZone(this);
        }
        //register aggro locations on adjacent cells
    }
}
