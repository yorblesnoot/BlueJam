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
    public CompassMaster compassMaster;

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
        compassMaster.transform.SetParent(null);
    }

    public IEnumerator ChainPath(List<GameObject> path)
    {
        foreach (GameObject tile in path)
        {
            WorldMovementController tileController = tile.GetComponent<WorldMovementController>();
            transform.LookAt(tileController.unitPosition);
            Vector3 displacement = tileController.unitPosition - transform.position;
            Vector3 cameraDestination = displacement + mainCamera.transform.position;

            //modify player's world position and run difficulty in run data
            Vector2Int newCoords = MapTools.VectorToMap(tile.transform.position);

            Vector2Int globalCoords = newCoords + WorldMapRenderer.spotlightGlobalOffset;

            runData.playerWorldX = globalCoords.x;
            runData.playerWorldY = globalCoords.y;
            runData.worldSteps++;

            EventManager.playerAtWorldLocation.Invoke(globalCoords);

            while (transform.position != tileController.unitPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, tileController.unitPosition, .05f);
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraDestination, .05f);
                fogRing.transform.position = Vector3.MoveTowards(fogRing.transform.position, tileController.unitPosition, .05f);
                yield return new WaitForSeconds(.02f);
            }
            


            yield return StartCoroutine(tileController.myEventHandler.TriggerWorldEvents());
        }
        playerState = WorldPlayerState.IDLE;
    }
}
