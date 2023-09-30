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
        cellEvent = null; cellEnemy = null;
    }

    void GlowIfAdjacentEnemy()
    {
        Vector2Int myPosition = MapTools.VectorToMap(transform.position);
        myPosition += WorldMapRenderer.spotlightGlobalOffset;
        List<Vector2Int> adjacents = myPosition.GetAdjacentCoordinates();
        adjacents.Add(myPosition);
        foreach (var adjacent in adjacents)
        {
            if (runData.eventMap.ContainsKey(adjacent) && runData.eventMap[adjacent] == EventType.ENEMY)
            { 
                redGlow.SetActive(true);
                Tutorial.Initiate(TutorialFor.WORLDBATTLE, TutorialFor.WORLDPICKUPS);
                Tutorial.EnterStage(TutorialFor.WORLDBATTLE, 1, "Red energy over a tile means that there's an enemy there! If I walk over one of those tiles, I'll go into battle!");
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
                WorldPlayerControl.playerState = WorldPlayerState.SELECTION;
                EventManager.updateItemUI.AddListener(() => pickedItem = true);
                yield return new WaitUntil(() => pickedItem == true);
                new SaveContainer(runData).SaveGame();
            }
            yield return new WaitForSeconds(.3f);
            cellEvent = null;
        }

        sceneRelay.bossEncounter = false;
        if (cellEnemy != null && cellEnemy.GetType() == typeof(WorldBoss))
        {
            Tutorial.CompleteStage(TutorialFor.WORLDBOSS, 1, true);
            sceneRelay.bossEncounter = true;
            yield return StartCoroutine(LaunchCombat(cellEnemy));
            yield break;
        }

        List<Vector2Int> adjacentPositions = MapTools.VectorToMap(transform.position).GetAdjacentCoordinates();
        adjacentPositions.Add(MapTools.VectorToMap(transform.position));
        foreach(Vector2Int position in adjacentPositions)
        {
            GameObject tile = MapTools.MapToTile(position);
            if(tile == null) continue;
            WorldEventHandler eventHandler = tile.GetComponent<WorldEventHandler>();
            if (eventHandler.cellEnemy == null || eventHandler.cellEnemy.GetType() == typeof(WorldBoss)) continue;
            Tutorial.CompleteStage(TutorialFor.WORLDBATTLE, 1, true);
            yield return StartCoroutine(LaunchCombat(eventHandler.cellEnemy));
            yield break;
            
        }
        EventManager.updateWorldCounters.Invoke();
    }

    IEnumerator LaunchCombat(WorldEnemy cellEnemy)
    {
        if (cellEnemy.GetType() == typeof(WorldBoss)) sceneRelay.spawnPool = cellEnemy.spawnPool;
        else sceneRelay.spawnPool = tileEnemyPreset;
        WorldEncounterBuilder builder = new(sceneRelay, runData);

        //remove activated enemies from the enemy map in rundata
        Vector2Int enemyCoords = MapTools.VectorToMap(cellEnemy.transform.position) + WorldMapRenderer.spotlightGlobalOffset;
        runData.eventMap.Remove(enemyCoords);
        WorldEventRenderer.spawnedEvents.Remove(enemyCoords);

        //modify encounters-- extra map generation parameters
        builder.ModifyMapGeneration(biomeMaps);

        yield return StartCoroutine(AnimatePlayerToBattle());
        //start combat
        builder.LaunchEncounter();
    }

    readonly static float descentTime = .4f;
    readonly static float randomDistanceWithinTile = .3f;
    readonly static float sparkleElevation = .2f;
    IEnumerator AnimatePlayerToBattle()
    {
        GameObject player = WorldPlayerControl.player.playerModel;
        Vector3 risePosition = player.transform.position;
        Vector3 dropPosition = transform.position;
        dropPosition.x += Random.Range(-randomDistanceWithinTile, randomDistanceWithinTile);
        dropPosition.z += Random.Range(-randomDistanceWithinTile, randomDistanceWithinTile);
        dropPosition.y += .2f;
        float timeElapsed = 0;
        WorldPlayerControl.player.GetComponent<UnitAnimator>().Animate(AnimType.JUMP);
        yield return new WaitForSeconds(.6f);
        Vector3 startScale = player.transform.localScale;
        while (timeElapsed < descentTime)
        {
            Vector3 step = Vector3.Lerp(risePosition, dropPosition, timeElapsed / descentTime);
            player.transform.position = step;

            Vector3 shrink = Vector3.Lerp(startScale, Vector3.zero, timeElapsed / descentTime);
            player.transform.localScale = shrink;
            timeElapsed += Time.deltaTime;
            
            yield return null;
        }
        player.gameObject.SetActive(false);
        Vector3 sparklePosition = dropPosition;
        sparklePosition.y += sparkleElevation;
        VFXMachine.PlayAtLocation("DistanceSparkle", sparklePosition);
        yield return new WaitForSeconds(1f);
    }

    public void RegisterEnemy(WorldEnemy enemy)
    {
        cellEnemy = enemy;
    }

    public void RegisterEvent(WorldEvent item)
    {
        cellEvent = item;
    }
}

