using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventRenderer : MonoBehaviour
{
    [SerializeField] RunData _runData;
    [SerializeField] EventSpawnRates _eventSpawnRates;
    static RunData runData;

    readonly float eventHeight = .7f;

    static Dictionary<EventType, ObjectPool> eventTable;
    public static Dictionary<Vector2Int, GameObject> spawnedEvents;

    private void Awake()
    {
        runData = _runData;
        spawnedEvents = new();
        eventTable = _eventSpawnRates.GetEventTable();
    }

    public GameObject RenderCellEvent(Vector2Int localCoordinates, Vector2Int offset)
    {
        Vector2Int globalCoordinates = localCoordinates + offset;
        if (!runData.eventMap.ContainsKey(globalCoordinates)) return null;
        EventType cellKey = runData.eventMap[globalCoordinates];
        Vector3 spawnLocation = MapTools.MapToVector(localCoordinates, eventHeight);
        GameObject cellEvent = eventTable[cellKey].InstantiateFromPool(spawnLocation, PhysicsHelper.RandomCardinalRotate());
        spawnedEvents.Add(globalCoordinates,cellEvent);
        return cellEvent;
    }

    public static void UnregisterCellEvent(Vector2Int globalCoordinates)
    {
        if (runData.eventMap.TryGetValue(globalCoordinates, out EventType eventType)
            && eventTable.TryGetValue(eventType, out ObjectPool pool)
            && spawnedEvents.TryGetValue(globalCoordinates, out GameObject eventObj))
        {
            eventObj.transform.parent = null;
            pool.ReturnToPool(eventObj);
            spawnedEvents.Remove(globalCoordinates);
        }
    }

    public void UnrenderCellEvent(Vector2Int globalCoordinates, float delay)
    {
        if (runData.eventMap.TryGetValue(globalCoordinates, out EventType eventType)
            && eventTable.TryGetValue(eventType, out ObjectPool pool)
            && spawnedEvents.TryGetValue(globalCoordinates, out GameObject eventObj))
        {
            StartCoroutine(pool.DesignateForPool(eventObj, delay));
            spawnedEvents.Remove(globalCoordinates);
        }
    }

    public static void MoveEvent(Vector2Int newPosition, Vector2Int oldPosition, EventType eventType, GameObject eventObject)
    {
        runData.eventMap.Remove(oldPosition);
        runData.eventMap.Add(newPosition, eventType);
        spawnedEvents.Remove(oldPosition);
        spawnedEvents.Add(newPosition, eventObject);
    }
}
