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

    void Awake()
    {
        playerState = WorldPlayerState.IDLE;
        Vector3 myPosition = MapTools.MapToVector(runData.playerWorldX, runData.playerWorldY, WorldMovementController.heightAdjust);
        gameObject.transform.position = myPosition;
        MapTools.playerLocation = MapTools.VectorToMap(myPosition);
    }

    public IEnumerator ChainPath(List<GameObject> path)
    {
        foreach (GameObject tile in path)
        {
            WorldMovementController tileController = tile.GetComponent<WorldMovementController>();
            if (tileController.eventHandler.enemyEvents.OfType<WorldBoss>().Any())
            {
                if (runData.KeyStock < 3)
                {
                    break;
                }
            }
                transform.LookAt(tileController.unitPosition);
            while (gameObject.transform.position != tileController.unitPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, tileController.unitPosition, .05f);
                yield return new WaitForSeconds(.01f);
            }
            //modify player's world position in run data
            Vector2Int newCoords = MapTools.VectorToMap(tile.transform.position);
            runData.playerWorldX = newCoords[0];
            runData.playerWorldY = newCoords[1];
            MapTools.playerLocation = newCoords;
            runData.worldSteps++;
            StartCoroutine(tileController.eventHandler.TriggerWorldEvents());
            yield return new WaitUntil(() => tileController.eventHandler.runningEvents == false);
        }
       playerState = WorldPlayerState.IDLE;
    }
}
