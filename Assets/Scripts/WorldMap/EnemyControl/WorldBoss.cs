using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoss : WorldEnemy
{
    public SpawnPool bossPool;
    public override void PullSpawnPool(WorldEventHandler handler)
    {
        //get static spawn
        spawnPool = bossPool;
    }
}
