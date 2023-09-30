using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventSpawnRates", menuName = "ScriptableObjects/EventSpawns")]
public class EventSpawnRates : ScriptableObject
{
    [SerializeField] List<EventKey> events;
    int totalChance;
    readonly List<int> probabilities;
    public void Initialize()
    {
        if (totalChance != 0) return;
        totalChance = 0;
        foreach (var e in events)
        {
            totalChance += e.spawnChance;
        }
    }

    public EventType RandomEvent()
    {
        int selection = Random.Range(0, totalChance);
        int currentThreshold = 0;
        foreach (var e in events)
        {
            currentThreshold += e.spawnChance;
            if (selection <= currentThreshold)
            {
                return e.type;
            }
        }
        return EventType.NONE;
    }

    public Dictionary<EventType, ObjectPool> GetEventTable()
    {
        Dictionary<EventType, ObjectPool> eventTable = new();
        foreach (var e in events)
        {
            eventTable.Add(e.type, new ObjectPool(e.obj));
        }
        return eventTable;
    }
}

[System.Serializable]
class EventKey
{
    public GameObject obj;
    public int spawnChance;
    public EventType type;
}
