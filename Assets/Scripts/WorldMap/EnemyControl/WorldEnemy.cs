using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemy : MonoBehaviour
{
    [HideInInspector]public SpawnPool spawnPool;
    private void OnEnable()
    {
        WorldEventHandler handler = MapTools.VectorToTile(transform.position).GetComponent<WorldEventHandler>();
        PullSpawnPool(handler);
        RegisterWithTile(handler);
    }
    public virtual void PullSpawnPool(WorldEventHandler handler)
    {
        //get spawn pool from tile we were spawned on
        spawnPool = handler.tileEnemyPreset;
    }

    public void RegisterWithTile(WorldEventHandler handler)
    {
        handler.cellEnemy = this;
    }
}
