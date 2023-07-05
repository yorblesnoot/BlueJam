using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventHandler : MonoBehaviour
{
    public SpawnPool tileEnemyPreset;
    public BiomePool biomeMaps;
    public RunData runData;
    public SceneRelay sceneRelay;

    [HideInInspector] public List<WorldEnemy> enemyEvents;

    //[HideInInspector] public List<EventEncounterModifier> modifierEvents;

    [HideInInspector] public WorldEvent cellEvent;

    bool pickedItem = false;
    public bool runningEvents = false;

    public IEnumerator TriggerWorldEvents()
    {
        runningEvents = true;
        //give the player whatever items
        if(cellEvent != null)
        {
            cellEvent.Activate();
            //if the event is an item, hold battle until an item is selected
            if(cellEvent.GetType() == typeof(ItemEvent))
            {
                EventManager.updateItemUI.AddListener(ConfirmItemPicked);
                yield return new WaitUntil(() => pickedItem == true);
                new SaveContainer(runData).SaveGame();
            }
            yield return new WaitForSeconds(.3f);
            cellEvent = null;
        }

        if (enemyEvents.Count > 0)
        {
            sceneRelay.bossEncounter = false;
            WorldEncounterBuilder builder = new(sceneRelay, runData);
            //consolidate enemy combats
            List<SpawnPool> pools = new();
            foreach (WorldEnemy enemy in enemyEvents)
            {
                if(enemy.GetType() == typeof(WorldBoss)) sceneRelay.bossEncounter = true;
                pools.Add(enemy.spawnPool);
                //remove activated enemies from the enemy map in rundata
                runData.worldEnemies?.RemoveCoordinates(MapTools.VectorToMap(enemy.gameObject.transform.position));
            }
            builder.ConsolidateSpawnPools(pools);

            //modify encounters-- extra map generation parameters
            builder.ModifyMapGeneration(biomeMaps);

            //start combat
            builder.LaunchEncounter();
        }
        EventManager.updateWorldCounters.Invoke();
        runningEvents = false;
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

