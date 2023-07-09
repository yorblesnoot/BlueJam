using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventRenderer : MonoBehaviour
{
    [SerializeField] GameObject bossEnemy;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject remove;
    [SerializeField] GameObject item;
    [SerializeField] GameObject heal;

    [SerializeField] RunData runData;

    readonly float eventHeight = .7f;
    public void RenderCellEvent(Vector2Int localCoordinates, Vector2Int offset)
    {
        Vector2Int globalCoordinates = localCoordinates + offset;
        Dictionary<string,GameObject> eventTable = new()
            {{ "e", enemy },
            { "r", remove },
            { "i", item },
            { "h", heal },
            { "b", bossEnemy }};
        if (!runData.eventMap.ContainsKey(globalCoordinates)) return;
        string cellKey = runData.eventMap[globalCoordinates];
        Vector3 spawnLocation = MapTools.MapToVector(localCoordinates.x, localCoordinates.y, eventHeight);
        Instantiate(eventTable[cellKey], spawnLocation, Quaternion.identity);
    }
}
