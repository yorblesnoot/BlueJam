using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalSpawn", menuName = "ScriptableObjects/Singletons/GlobalSpawn")]
public class GlobalSpawn : ScriptableObject
{
    enum UnitType
    {
        STANDARD,
        MINION,
        BOSS
    }
    [SerializeField] List<EnemySpawn> spawns;

    [System.Serializable]
    class EnemySpawn
    {
        public GameObject spawnableUnit;
        public UnitType type;
        public TerrainType[] biomes;
    }
}
