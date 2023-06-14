using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventHandler : MonoBehaviour
{
    public SpawnPool tileEnemyPreset;
    public BiomePool biomeMaps;
    public RunData runData;

    [HideInInspector] public List<WorldEnemy> enemyEvents;

    //[HideInInspector] public List<EventEncounterModifier> modifierEvents;

    [HideInInspector] public WorldEvent cellEvent;

    bool pickedItem = false;

    public IEnumerator TriggerWorldEvents()
    {
        yield return new WaitForSeconds(.5f);
        //give the player whatever items
        if(cellEvent != null)
        {
            cellEvent.Activate();
            //if the event is an item, hold battle until an item is selected
            if(cellEvent.GetType() == typeof(ItemEvent))
            {
                EventManager.updateItemUI.AddListener(ConfirmItemPicked);
                yield return new WaitUntil(() => pickedItem == true);
            }
            yield return new WaitForSeconds(.5f);
            cellEvent = null;
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
                if(runData.worldEnemies != null) runData.worldEnemies.RemoveCoordinates(GridTools.VectorToMap(enemy.gameObject.transform.position));
            }
            builder.ConsolidateSpawnPools(pools);

            //modify encounters-- extra map generation parameters
            builder.ModifyMapGeneration(biomeMaps);

            //start combat
            builder.LaunchEncounter();
        }
        else
        {
            //if combat didnt happen, allow a new player movement
            EventManager.getWorldDestination?.Invoke(runData.playerWorldX, runData.playerWorldY);
        }
    }

    public void ConfirmItemPicked()
    {
        pickedItem = true;
    }

    public void RegisterAggroZone(WorldEnemy enemy)
    {
        enemyEvents.Add(enemy);
    }

    public void RegisterEvent(WorldEvent item)
    {
        cellEvent = item;
    }
}

