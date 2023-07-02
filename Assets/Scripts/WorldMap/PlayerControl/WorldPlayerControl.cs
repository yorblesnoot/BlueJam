using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldPlayerControl : MonoBehaviour
{
    public RunData runData;
    public float heightAdjust;

    void Awake()
    {
        heightAdjust = 1f;
        EventManager.worldMove.AddListener(RequestValidMoves);
        Vector3 myPosition = MapTools.MapToVector(runData.playerWorldX, runData.playerWorldY, heightAdjust);
        gameObject.transform.position = myPosition;
    }

    public void RequestValidMoves()
    {
        //tell adjacent tiles to light up for a move
        EventManager.getWorldDestination?.Invoke(runData.playerWorldX, runData.playerWorldY);
    }

    public IEnumerator MoveToWorldCell(GameObject cell)
    {
        WorldMovementController movementController = cell.GetComponent<WorldMovementController>();
        transform.LookAt(movementController.unitPosition);
        Vector3 moveTarget = movementController.unitPosition;
        while (gameObject.transform.position != moveTarget)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, moveTarget, .02f);
            yield return new WaitForSeconds(.01f);
        }

        //modify player's world position in run data
        Vector2Int newCoords = MapTools.VectorToMap(cell.transform.position);
        runData.playerWorldX = newCoords[0];
        runData.playerWorldY = newCoords[1];

        StartCoroutine(cell.GetComponent<WorldEventHandler>().TriggerWorldEvents());
    }
}
