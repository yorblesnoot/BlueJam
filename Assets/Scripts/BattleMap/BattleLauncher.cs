using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

//public class HighlightCells : UnityEvent<int, int, string, GameObject> { }

public class BattleLauncher : MonoBehaviour
{
#nullable enable
    public static string[,]? battleMap;
#nullable disable

    public GenerationParameters generationParameters;
    public GameObject player;
    public RunData runData;

    private void Start() 
    { 
        //generate map based on runData from world map
        ProceduralMapGenerator procedural = new ProceduralMapGenerator();
        battleMap = procedural.Generate(runData.battleParameters);
        if(battleMap == null) 
        {
            battleMap = new string[,] { { "n", "n" }, { "n", "n" } };
        }

        //render the map
        GetComponent<MapRenderer>().RenderWorld(battleMap);

        //place units onto the map
        BattleUnitSpawner encounterBuilder = new BattleUnitSpawner(runData.staticSpawns, runData.spawnUnits, runData.spawnWeights, battleMap);
        encounterBuilder.PlaceUnit(player);
        encounterBuilder.PlaceEnemies(runData.enemyBudget);

        //activate item effects
        foreach(BattleItem item in runData.itemInventory)
        {
            item.effect.Execute(player, GridTools.VectorToTile(player.transform.position), new string [,]{ { "n"} });
        }

        //initialize combat
        EventManager.initalizeBattlemap?.Invoke();
    }
}
