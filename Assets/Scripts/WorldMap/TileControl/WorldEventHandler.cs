using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventHandler : MonoBehaviour
{
    public SpawnPool tileEnemyPreset;
    public GenerationParameters generationParameters;
    public RunData runData;

    [HideInInspector] public List<WorldEnemy> enemyEvents;

    //[HideInInspector] public List<EventEncounterModifier> modifierEvents;

    [HideInInspector] public WorldEvent itemEvent;
    public IEnumerator TriggerWorldEvents()
    {
        yield return new WaitForSeconds(.5f);
        //give the player whatever items
        if(itemEvent != null)
        {
            itemEvent.Activate();
            itemEvent = null;
            yield return new WaitForSeconds(.5f);
        }

        if (enemyEvents.Count > 0)
        {
            runData.bossEncounter = false;
            WorldEncounterBuilder builder = new(runData);
            //consolidate enemy combats
            List<SpawnPool> pools = new();
            foreach (WorldEnemy enemy in enemyEvents)
            {
                if(enemy.GetType() == typeof(WorldBoss)) runData.bossEncounter = true;
                pools.Add(enemy.spawnPool);
                //remove activated enemies from the enemy map in rundata
                if(runData.worldEnemies != null) runData.worldEnemies.Remove(GridTools.VectorToMap(enemy.gameObject.transform.position));
            }
            builder.ConsolidateSpawnPools(pools);

            //modify encounters-- extra map generation parameters
            builder.ModifyMapGeneration(generationParameters);

            //start combat
            builder.LaunchEncounter();
        }
        else
        {
            //if combat didnt happen, allow a new player movement
            EventManager.getWorldDestination?.Invoke(runData.playerWorldX, runData.playerWorldY);
        }
    }

    public void RegisterAggroZone(WorldEnemy enemy)
    {
        enemyEvents.Add(enemy);
    }

    public void RegisterEvent(WorldEvent item)
    {
        itemEvent = item;
    }
}

