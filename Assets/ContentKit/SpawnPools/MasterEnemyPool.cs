using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MasterPool", menuName = "ScriptableObjects/Singletons/MasterPool")]
public class MasterEnemyPool : ScriptableObject
{
    [SerializeField] List <GameObject> weight1Pool;
    [SerializeField] List <GameObject> weight2Pool;

    public Dictionary<GameObject, int> masterPool;
    public void Initialize()
    {
        masterPool = new();
        AddToMaster(weight1Pool, 1);
        AddToMaster(weight2Pool, 2);
    }

    void AddToMaster(List<GameObject> pool, int value)
    {
        foreach (var enemy in pool)
            masterPool.Add(enemy, value);
    }
}
