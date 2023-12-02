using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum WorldPlayerState { IDLE, PATHING, MENUS, SELECTION}
public enum VehicleMode
{
    NONE,
    BOAT,
    BALLOON
}
public class WorldPlayerControl : MonoBehaviour
{
    public RunData runData;

    public static WorldPlayerState playerState;

    public static VehicleEvent CurrentVehicle;

    public GameObject playerVisual;
    public GameObject playerModel;
    [SerializeField] WorldMapRenderer worldRenderer;
    [SerializeField] Animatable unitAnimator;
    public CompassMaster compassMaster;
    public static WorldPlayerControl player;

    
    public static Vector3 playerBaseScale;
    public void InitializePlayer()
    {
        player = this;
        playerBaseScale = playerModel.transform.localScale;
        playerState = WorldPlayerState.IDLE;
        Vector2Int worldMapCoords = new(runData.playerWorldX, runData.playerWorldY);
        worldMapCoords -= WorldMapRenderer.spotlightGlobalOffset;
        Vector3 myPosition = MapTools.MapToVector(worldMapCoords, WorldMovementController.heightAdjust);
        gameObject.transform.position = myPosition;
    }
    readonly public static float moveTime = .5f;
    public IEnumerator ChainPath(List<Vector2Int> path)
    {
        new SaveContainer(runData).SaveGame();
        Tutorial.CompleteStage(WorldEventHandler.lastTutorial, 1, true);

        if(CurrentVehicle == null) unitAnimator.Animate(AnimType.WALK);
        for (int i = 0; i < path.Count; i++)
        {
            if (CurrentVehicle == null && i == path.Count - 1) { StartCoroutine(unitAnimator.EndWalk(moveTime)); }
            Vector2Int tile = path[i];
            Vector2Int globalCoords = tile + WorldMapRenderer.spotlightGlobalOffset;
            if (RunStarter.unpathable.Contains(runData.worldMap[globalCoords.x, globalCoords.y]) 
                && CurrentVehicle == null)
                if(!(runData.eventMap.TryGetValue(globalCoords, out var value) && (value == EventType.BOAT || value == EventType.BALLOON))) 
                    yield break;

            playerState = WorldPlayerState.PATHING;
            runData.score -= 1;
            WorldMovementController tileController = MapTools.MapToTile(tile).GetComponent<WorldMovementController>();
            playerVisual.transform.LookAt(tileController.unitPosition);
            if (tileController.myEventHandler.cellEvent != null) tileController.myEventHandler.cellEvent.PreAnimate();

            //modify player's world position and run difficulty in run data
            Vector2Int oldCoords = MapTools.VectorToMap(transform.position) + WorldMapRenderer.spotlightGlobalOffset;

            runData.playerWorldX = globalCoords.x;
            runData.playerWorldY = globalCoords.y;
            runData.worldSteps++;

            if (CurrentVehicle != null) CurrentVehicle.CheckForDismount(globalCoords);

            EventManager.playerAtWorldLocation.Invoke(globalCoords);

            float timeElapsed = 0;
            Vector3 startPosition = transform.position;
            while (timeElapsed < moveTime)
            {
                Vector3 step = Vector3.Lerp(startPosition, tileController.unitPosition, timeElapsed / moveTime);
                timeElapsed += Time.deltaTime;
                transform.position = step;
                yield return null;
            }
            SoundManager.PlaySound(SoundType.SLIMESTEP);
            bool onVehicle = false;
            if (CurrentVehicle != null) onVehicle = true;
            tileController.ClearHighlight();
            yield return StartCoroutine(tileController.myEventHandler.TriggerWorldEvents());
            if (onVehicle) CurrentVehicle.RelocateVehicle(globalCoords, oldCoords);
            playerState = WorldPlayerState.IDLE;
        }
        WorldMovementController.pathingComplete.Invoke();
    }
}
