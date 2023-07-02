using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

//public class HighlightCells : UnityEvent<int, int, string, GameObject> { }

public class BattleLauncher : MapLauncher
{
#nullable enable
    public static string[,]? battleMap;
#nullable disable

    public GameObject player;
    public RunData runData;

    [SerializeField] CameraLock camLock;

    private void Start() 
    {
        //instantiate a prefab map
        Instantiate(runData.availableMaps.DispenseMap(), new Vector3(0, 0, 0), Quaternion.identity);

        //tell the camera to find the lockpoint on the battle map and lock onto it
        camLock.CameraLockOn();
        RequestMapReferences();

        //place units onto the map
        BattleUnitSpawner encounterBuilder = new BattleUnitSpawner(runData.staticSpawns, runData.spawnUnits, runData.spawnWeights, map);
        encounterBuilder.PlacePlayer(player);
        PlayerUnit playerUnit = player.GetComponent<PlayerUnit>();
        MapTools.ReportPlayer(playerUnit, playerUnit.transform.position);
        encounterBuilder.PlaceEnemies(runData.enemyBudget);

        //initialize combat
        EventManager.initalizeBattlemap?.Invoke();

        //activate item effects
        foreach (BattleItem item in runData.itemInventory)
        {
            foreach (var effect in item.effects)
            {
                effect.Execute(playerUnit, MapTools.VectorToTile(player.transform.position).GetComponent<BattleTileController>());
            }
        }
    }
}
