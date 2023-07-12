using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldEventHandler : MonoBehaviour
{
    public SpawnPool tileEnemyPreset;
    public List<GameObject> biomeMaps;
    public RunData runData;
    public SceneRelay sceneRelay;

    [HideInInspector] public WorldEvent cellEvent;
    [HideInInspector] public WorldEnemy cellEnemy;

    bool pickedItem = false;
    [SerializeField] GameObject redGlow;

    private void OnEnable()
    {
        GlowIfAdjacentEnemy();
    }

    void GlowIfAdjacentEnemy()
    {
        Vector2Int myPosition = MapTools.VectorToMap(transform.position);
        myPosition += WorldMapRenderer.spotlightGlobalOffset;
        List<Vector2Int> adjacents = myPosition.GetAdjacentCoordinates();
        adjacents.Add(myPosition);
        foreach (var adjacent in adjacents)
        {
            if (runData.eventMap.ContainsKey(adjacent) && runData.eventMap[adjacent] == "e")
            { 
                redGlow.SetActive(true);
                return;
            }
        }
        redGlow.SetActive(false);
    }
    public IEnumerator TriggerWorldEvents()
    {
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

        sceneRelay.bossEncounter = false;
        if (cellEnemy != null && cellEnemy.GetType() == typeof(WorldBoss))
        {
            sceneRelay.bossEncounter = true;
            LaunchCombat(cellEnemy);
            Debug.Log(cellEnemy.GetType());
            Debug.Log(cellEnemy.name);
        }

        List<Vector2Int> adjacentPositions = MapTools.VectorToMap(transform.position).GetAdjacentCoordinates();
        foreach(Vector2Int position in adjacentPositions)
        {
            GameObject tile = MapTools.MapToTile(position);
            if(tile == null) continue;
            WorldEventHandler eventHandler = tile.GetComponent<WorldEventHandler>();
            if (eventHandler.cellEnemy == null || eventHandler.cellEnemy.GetType() == typeof(WorldBoss)) continue;
            LaunchCombat(eventHandler.cellEnemy);
            
        }
        EventManager.updateWorldCounters.Invoke();
    }

    void LaunchCombat(WorldEnemy cellEnemy)
    {
        sceneRelay.spawnPool = cellEnemy.spawnPool;
        WorldEncounterBuilder builder = new(sceneRelay, runData);

        //remove activated enemies from the enemy map in rundata
        runData.eventMap.Remove(MapTools.VectorToMap(cellEnemy.transform.position) + WorldMapRenderer.spotlightGlobalOffset);

        //modify encounters-- extra map generation parameters
        builder.ModifyMapGeneration(biomeMaps);

        //start combat
        builder.LaunchEncounter();
    }

    public void ConfirmItemPicked() { pickedItem = true; }

    public void RegisterEnemy(WorldEnemy enemy)
    {
        cellEnemy = enemy;
    }

    public void RegisterEvent(WorldEvent item)
    {
        cellEvent = item;
    }
}

