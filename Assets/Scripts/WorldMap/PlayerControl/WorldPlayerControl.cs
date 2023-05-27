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
        Vector3 myPosition = GridTools.MapToVector(runData.worldX, runData.worldY, heightAdjust);
        gameObject.transform.position = myPosition;
    }

    public void RequestValidMoves()
    {
        //tell adjacent tiles to light up for a move
        EventManager.getWorldDestination?.Invoke(runData.worldX, runData.worldY);
    }

    public IEnumerator MoveToWorldCell(GameObject cell)
    {
        Vector3 moveTarget = cell.transform.position;
        moveTarget = new Vector3(moveTarget.x, moveTarget.y + heightAdjust, moveTarget.z);
        while (gameObject.transform.position != moveTarget)
        {

            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, moveTarget, .02f);
            yield return new WaitForSeconds(.02f);
        }

        //modify player's world position in run data
        int[] newCoords = GridTools.VectorToMap(cell.transform.position);
        runData.worldX = newCoords[0];
        runData.worldY = newCoords[1];

        StartCoroutine(cell.GetComponent<WorldEventHandler>().TriggerWorldEvents());
    }
}
