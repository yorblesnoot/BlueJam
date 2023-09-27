using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] UnitAnimator unitAnimator;
    public CompassMaster compassMaster;
    public static WorldPlayerControl player;

    readonly public static float moveTime = .5f;
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

    public IEnumerator ChainPath(List<GameObject> path)
    {
        Tutorial.CompleteStage(TutorialFor.WORLDHEAL, 1, true);
        Tutorial.CompleteStage(TutorialFor.WORLDITEM, 1, true);
        Tutorial.CompleteStage(TutorialFor.WORLDREMOVE, 1, true);
        foreach (GameObject tile in path)
        {
            playerState = WorldPlayerState.PATHING;
            runData.score -= 1;
            WorldMovementController tileController = tile.GetComponent<WorldMovementController>();
            playerVisual.transform.LookAt(tileController.unitPosition);
            if (tileController.myEventHandler.cellEvent != null) tileController.myEventHandler.cellEvent.PreAnimate();



            //modify player's world position and run difficulty in run data
            Vector2Int oldCoords = MapTools.VectorToMap(transform.position) + WorldMapRenderer.spotlightGlobalOffset;

            Vector2Int globalCoords = MapTools.VectorToMap(tile.transform.position) + WorldMapRenderer.spotlightGlobalOffset;

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
            yield return StartCoroutine(tileController.myEventHandler.TriggerWorldEvents());
            if (onVehicle) CurrentVehicle.RelocateVehicle(globalCoords, oldCoords);


            playerState = WorldPlayerState.IDLE;
        }
        
    }
}
