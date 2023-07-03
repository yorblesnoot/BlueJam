using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEncounterBuilder
{
    public RunData RunData;
    public SceneRelay sceneRelay;
    public WorldEncounterBuilder(SceneRelay relay, RunData data)
    {
        sceneRelay = relay;
        RunData = data;
    }

    public void ConsolidateSpawnPools(List<SpawnPool> pools)
    {
        sceneRelay.spawnUnits = new();
        sceneRelay.spawnWeights = new();
        sceneRelay.staticSpawns = new();
        foreach (SpawnPool pool in pools)
        {
            sceneRelay.spawnUnits.AddRange(pool.spawnUnits);
            sceneRelay.spawnWeights.AddRange(pool.spawnWeight);
            sceneRelay.staticSpawns.AddRange(pool.staticSpawns);
        }
    }

    public void ModifyMapGeneration(BiomePool maps)
    {
        sceneRelay.availableMaps = maps;
    }

    public void LaunchEncounter()
    {
        //save the biome generation data to runData, then send us into the battlemap
        sceneRelay.enemyBudget = 4 + RunData.runDifficulty/2;
        EventManager.prepareForBattle.Invoke();
        SceneManager.LoadScene(2);
    }
}
