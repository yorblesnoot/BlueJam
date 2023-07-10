using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MasterPool", menuName = "ScriptableObjects/Singletons/MasterPool")]
public class MasterEnemyPool : ScriptableObject
{
    [SerializeField] List <GameObject> weight1Pool;
    [SerializeField] List <GameObject> weight2Pool;
    [SerializeField] List <GameObject> weight3Pool;
    [SerializeField] List <GameObject> weight4Pool;

    public Dictionary<GameObject, int> masterPool;
    public void Initialize()
    {
        masterPool = new();
        AddToMaster(weight1Pool, 1);
        AddToMaster(weight2Pool, 2);
        AddToMaster(weight3Pool, 3);
        AddToMaster(weight4Pool, 4);
    }

    void AddToMaster(List<GameObject> pool, int value)
    {
        foreach (var enemy in pool)
            masterPool.Add(enemy, value);
    }
}
