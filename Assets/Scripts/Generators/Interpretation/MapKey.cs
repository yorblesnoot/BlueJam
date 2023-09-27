using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapKey", menuName = "ScriptableObjects/MapKit/MapKey")]
public class MapKey : ScriptableObject
{
    [System.Serializable]
    class TerrainObj
    {
        public TerrainType terrain;
        public GameObject obj;
    }
    public Dictionary<TerrainType, GameObject> hashKey;
    [SerializeField] List<TerrainObj> terrainKey;

    public void Initialize()
    {
        hashKey = new();
        for (int i = 0; i < terrainKey.Count; i++)
        {
            hashKey.Add(terrainKey[i].terrain, terrainKey[i].obj);
        }
    }
}
