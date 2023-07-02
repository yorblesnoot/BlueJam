using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoss : WorldEnemy
{
    public SpawnPool bossPool;
    public override void PullSpawnPool()
    {
        //get static spawn
        spawnPool = bossPool;
    }

    public override void RegisterAggroZone()
    {
        //register aggro on cell
        MapTools.VectorToTile(transform.position).GetComponent<WorldEventHandler>().RegisterAggroZone(this);
    }
}
