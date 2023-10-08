using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class WorldEventHandler : MonoBehaviour
{
    public SpawnPool tileEnemyPreset;
    public List<GameObject> biomeMaps;
    public RunData runData;
    public SceneRelay sceneRelay;

    [HideInInspector] public WorldEvent cellEvent;
    [HideInInspector] public WorldEnemy cellEnemy;

    public bool eventComplete;
    [SerializeField] GameObject redGlow;

    public static TutorialFor lastTutorial;

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
                Tutorial.EnterStage(TutorialFor.WORLDBATTLE, 1, "Red energy over a tile means that there's an <color=red>enemy</color> there! If I walk over one of those tiles, I'll go into <color=red>battle</color>!");
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
            eventComplete = false;
            cellEvent.Activate(this);
            WorldPlayerControl.playerState = WorldPlayerState.SELECTION;
            yield return new WaitUntil(() => eventComplete == true);

            
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
            Vector2Int global = position + WorldMapRenderer.spotlightGlobalOffset;
            if (RunStarter.unpathable.Contains(runData.worldMap[global.x, global.y]))
            {
                Tutorial.Initiate(TutorialFor.WORLDIMPASSABLE, TutorialFor.WORLDMOVE);
                Tutorial.EnterStage(TutorialFor.WORLDIMPASSABLE, 1, "Water and mountains are <color=red>impassable terrain</color>! But with a little exploration, you might find <color=green>vehicles</color> that will allow me to cross.");
            }
            GameObject tile = MapTools.MapToTile(position);
            if(tile == null) continue;
            WorldEventHandler eventHandler = tile.GetComponent<WorldEventHandler>();
            if (eventHandler.cellEnemy == null || eventHandler.cellEnemy.GetType() == typeof(WorldBoss)) continue;
            Tutorial.CompleteStage(TutorialFor.WORLDBATTLE, 1, true);
            //dont save game after events if we are entering combat; save should reload us on the previous tile
            yield return StartCoroutine(LaunchCombat(eventHandler.cellEnemy));
            yield break;
            
        }
        EventManager.updateWorldCounters.Invoke();
        WorldPlayerControl.playerState = WorldPlayerState.IDLE;
    }

    IEnumerator LaunchCombat(WorldEnemy cellEnemy)
    {
        if (cellEnemy.GetType() == typeof(WorldBoss)) sceneRelay.spawnPool = cellEnemy.spawnPool;
        else sceneRelay.spawnPool = tileEnemyPreset;

        //remove activated enemies from the enemy map in rundata
        Vector2Int enemyCoords = MapTools.VectorToMap(cellEnemy.transform.position) + WorldMapRenderer.spotlightGlobalOffset;
        runData.eventMap.Remove(enemyCoords);
        WorldEventRenderer.spawnedEvents.Remove(enemyCoords);

        yield return StartCoroutine(AnimatePlayerToBattle());
        //start combat
        LaunchEncounter();
    }

    void LaunchEncounter()
    {
        //save the biome generation data to runData, then send us into the battlemap
        sceneRelay.availableMaps = biomeMaps;
        sceneRelay.enemyBudget = Settings.Balance.BaseFoeBudget + runData.ThreatLevel / Settings.Balance.ThreatPerBudgetUp;
        EventManager.prepareForBattle.Invoke();
        EventManager.loadSceneWithScreen.Invoke(2);
        EventManager.loadSceneWithScreen.Invoke(-1);
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

