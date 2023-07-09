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

    DynamicEventPlacer dynamicEventPlacer;

    void Start()
    {
        playerState = WorldPlayerState.IDLE;
        Vector2Int localCoords = new(runData.playerWorldX, runData.playerWorldY);
        localCoords -= WorldMapRenderer.spotlightGlobalOffset;
        Vector3 myPosition = MapTools.MapToVector(localCoords.x, localCoords.y, WorldMovementController.heightAdjust);
        gameObject.transform.position = myPosition;
        MapTools.playerLocation = MapTools.VectorToMap(myPosition);
        fogRing.transform.SetParent(null);
        dynamicEventPlacer = new(runData);
        dynamicEventPlacer.CheckToPopulateChunks(MapTools.playerLocation + WorldMapRenderer.spotlightGlobalOffset);
    }

    public IEnumerator ChainPath(List<GameObject> path)
    {
        foreach (GameObject tile in path)
        {
            Vector2Int oldCoords = MapTools.VectorToMap(gameObject.transform.position);
            WorldMovementController tileController = tile.GetComponent<WorldMovementController>();
            if (tileController.eventHandler.enemyEvents.OfType<WorldBoss>().Any())
            {
                if (runData.KeyStock < 3)
                {
                    break;
                }
            }
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
            MapTools.playerLocation = newCoords;
            runData.worldSteps++;

            //draw new tiles
            worldRenderer.ShiftWindow(newCoords, newCoords - oldCoords);

            //check if we need to generate more world events
            dynamicEventPlacer.CheckToPopulateChunks(globalCoords);

            yield return StartCoroutine(tileController.eventHandler.TriggerWorldEvents());
        }
       playerState = WorldPlayerState.IDLE;
    }
}
