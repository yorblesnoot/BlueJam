using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEncounterBuilder
{
    public RunData RunData;
    public SceneRelay sceneRelay;
    readonly int baseEnemies = 4;
    public WorldEncounterBuilder(SceneRelay relay, RunData data)
    {
        sceneRelay = relay;
        RunData = data;
    }

    public void ModifyMapGeneration(List<GameObject> maps)
    {
        sceneRelay.availableMaps = maps;
    }

    public void LaunchEncounter()
    {
        //save the biome generation data to runData, then send us into the battlemap
        sceneRelay.enemyBudget = baseEnemies + RunData.runDifficulty/2;
        EventManager.prepareForBattle.Invoke();
        SceneManager.LoadScene(2);
    }
}
