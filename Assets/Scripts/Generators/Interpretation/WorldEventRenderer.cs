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
    [SerializeField] RunData runData;

    readonly float eventHeight = .7f;

    Dictionary<EventType, ObjectPool> eventTable;
    Dictionary<Vector2Int, GameObject> spawnedEvents;

    private void Awake()
    {
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
        GameObject cellEvent = eventTable[cellKey].InstantiateFromPool(spawnLocation, Quaternion.identity);
        spawnedEvents.Add(globalCoordinates,cellEvent);
        return cellEvent;
    }

    public void UnrenderCellEvent(Vector2Int globalCoordinates)
    {
        if (runData.eventMap.TryGetValue(globalCoordinates, out EventType eventType)
            && eventTable.TryGetValue(eventType, out ObjectPool pool)
            && spawnedEvents.TryGetValue(globalCoordinates, out GameObject eventObj))
        {
            pool.ReturnToPool(eventObj);
            spawnedEvents.Remove(globalCoordinates);
        }
    }

}
