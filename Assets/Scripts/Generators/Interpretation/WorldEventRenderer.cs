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

    Dictionary<string, ObjectPool> eventTable;
    Dictionary<Vector2Int, GameObject> spawnedEvents;

    private void Awake()
    {
        spawnedEvents = new();
        eventTable = new()
            {{ "e", new ObjectPool(enemy) },
            { "r",  new ObjectPool(remove) },
            { "i", new ObjectPool(item) },
            { "h", new ObjectPool(heal) },
            { "b", new ObjectPool(bossEnemy) }};
    }

    public GameObject RenderCellEvent(Vector2Int localCoordinates, Vector2Int offset)
    {
        Vector2Int globalCoordinates = localCoordinates + offset;
        if (!runData.eventMap.ContainsKey(globalCoordinates)) return null;
        string cellKey = runData.eventMap[globalCoordinates];
        Vector3 spawnLocation = MapTools.MapToVector(localCoordinates, eventHeight);
        GameObject cellEvent = eventTable[cellKey].InstantiateFromPool(spawnLocation, Quaternion.identity);
        spawnedEvents.Add(globalCoordinates,cellEvent);
        return cellEvent;
    }

    public void UnrenderCellEvent(Vector2Int globalCoordinates)
    {
        if (runData.eventMap.TryGetValue(globalCoordinates, out string eventType)
            && eventTable.TryGetValue(eventType, out ObjectPool pool)
            && spawnedEvents.TryGetValue(globalCoordinates, out GameObject eventObj))
        {
            pool.ReturnToPool(eventObj);
            spawnedEvents.Remove(globalCoordinates);
        }
    }

}
