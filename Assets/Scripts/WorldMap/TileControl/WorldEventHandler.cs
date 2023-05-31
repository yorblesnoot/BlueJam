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
        yield return new WaitForSeconds(1f);
        //give the player whatever items
        if(itemEvent != null )
        {
            itemEvent.Activate();
            itemEvent = null;
        }

        if (enemyEvents.Count > 0)
        {
            WorldEncounterBuilder builder = new(runData);
            //consolidate enemy combats
            List<SpawnPool> pools = new();
            foreach (WorldEnemy enemy in enemyEvents)
            {
                pools.Add(enemy.spawnPool);
                //remove activated enemies from the enemy map in rundata
                runData.worldEnemies.Remove(GridTools.VectorToMap(enemy.gameObject.transform.position));
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
            EventManager.getWorldDestination?.Invoke(runData.worldX, runData.worldY);
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

