using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventRenderer : MonoBehaviour
{
    [System.Serializable]
    class EventPool
    {
        public EventType eventType;
        [SerializeField] GameObject obj;
        [HideInInspector] public ObjectPool pool 
        { get { return new ObjectPool(obj); } }
    }

    [SerializeField] List<EventPool> pools;
    [SerializeField] RunData _runData;
    static RunData runData;

    readonly float eventHeight = .7f;

    static Dictionary<EventType, ObjectPool> eventTable;
    public static Dictionary<Vector2Int, GameObject> spawnedEvents;

    private void Awake()
    {
        runData = _runData;
        spawnedEvents = new();
        eventTable = new();
        foreach (EventPool pool in pools)
        {
            eventTable.Add(pool.eventType, pool.pool);
        }
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

    public static void UnrenderCellEvent(Vector2Int globalCoordinates)
    {
        if (runData.eventMap.TryGetValue(globalCoordinates, out EventType eventType)
            && eventTable.TryGetValue(eventType, out ObjectPool pool)
            && spawnedEvents.TryGetValue(globalCoordinates, out GameObject eventObj))
        {
            pool.ReturnToPool(eventObj);
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
