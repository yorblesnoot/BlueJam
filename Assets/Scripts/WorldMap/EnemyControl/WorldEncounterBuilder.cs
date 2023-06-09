using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEncounterBuilder
{
    public RunData runData;
    public GenerationParameters generationParameters;
    public WorldEncounterBuilder(RunData run)
    {
        runData = run;
    }

    public void ConsolidateSpawnPools(List<SpawnPool> pools)
    {
        runData.spawnUnits = new();
        runData.spawnWeights = new();
        runData.staticSpawns = new();
        foreach (SpawnPool pool in pools)
        {
            runData.spawnUnits.AddRange(pool.spawnUnits);
            runData.spawnWeights.AddRange(pool.spawnWeight);
            runData.staticSpawns.AddRange(pool.staticSpawns);
        }
    }

    public void ModifyMapGeneration(GenerationParameters parameters)
    {
        generationParameters = parameters;
    }

    public void LaunchEncounter()
    {
        //save the biome generation data to runData, then send us into the battlemap
        runData.battleParameters = generationParameters;
        runData.enemyBudget = runData.baseEnemies + runData.runDifficulty;
        SceneManager.LoadScene(2);
    }
}
