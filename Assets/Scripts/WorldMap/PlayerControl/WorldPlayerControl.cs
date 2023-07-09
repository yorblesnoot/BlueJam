using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public enum WorldPlayerState { IDLE, PATHING, MENUS}

public class WorldPlayerControl : MonoBehaviour
{
    public RunData runData;

    public static WorldPlayerState playerState;

    [SerializeField] GameObject fogRing;
    [SerializeField] Camera mainCamera;
    [SerializeField] WorldMapRenderer worldRenderer;
    [SerializeField] WorldCompass compass;
    DynamicEventPlacer dynamicEventPlacer;

    [SerializeField] Vector2Int worldBossLocation;

    [SerializeField] public static GameObject player;

    public void InitializePlayer()
    {
        player = gameObject;
        playerState = WorldPlayerState.IDLE;
        Vector2Int worldMapCoords = new(runData.playerWorldX, runData.playerWorldY);
        worldMapCoords -= WorldMapRenderer.spotlightGlobalOffset;
        Vector3 myPosition = MapTools.MapToVector(worldMapCoords, WorldMovementController.heightAdjust);
        gameObject.transform.position = myPosition;

        fogRing.transform.SetParent(null);
        dynamicEventPlacer = new(runData);
        dynamicEventPlacer.CheckToPopulateChunks(MapTools.VectorToMap(transform.position) + WorldMapRenderer.spotlightGlobalOffset);
        worldBossLocation = runData.eventMap.FirstOrDefault(x => x.Value == "b").Key;
    }

    public IEnumerator ChainPath(List<GameObject> path)
    {
        foreach (GameObject tile in path)
        {
            Vector2Int oldCoords = MapTools.VectorToMap(gameObject.transform.position);
            WorldMovementController tileController = tile.GetComponent<WorldMovementController>();
            transform.LookAt(tileController.unitPosition);
            Vector3 displacement = tileController.unitPosition - transform.position;
            Vector3 cameraDestination = displacement + mainCamera.transform.position;
            Vector3 fogDestination = displacement + fogRing.transform.position;

            while (transform.position != tileController.unitPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, tileController.unitPosition, .05f);
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraDestination, .05f);
                fogRing.transform.position = Vector3.MoveTowards(fogRing.transform.position, fogDestination, .05f);
                yield return new WaitForSeconds(.02f);
            }
            //modify player's world position and run difficulty in run data
            Vector2Int newCoords = MapTools.VectorToMap(tile.transform.position);

            Vector2Int globalCoords = newCoords+WorldMapRenderer.spotlightGlobalOffset;

            runData.playerWorldX = globalCoords.x;
            runData.playerWorldY = globalCoords.y;
            runData.worldSteps++;

            //draw new tiles
            worldRenderer.ShiftWindow(newCoords, newCoords - oldCoords);

            //check if we need to generate more world events
            dynamicEventPlacer.CheckToPopulateChunks(globalCoords);

            compass.PointAtWorldCoordinates(worldBossLocation, transform.position);

            yield return StartCoroutine(tileController.eventHandler.TriggerWorldEvents());
        }
        playerState = WorldPlayerState.IDLE;
    }
}
