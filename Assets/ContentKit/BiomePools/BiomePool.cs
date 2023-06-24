using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomePool", menuName = "ScriptableObjects/BiomePool")]
public class BiomePool : ScriptableObject
{
    public List<GameObject> maps;

    public GameObject DispenseMap()
    {
        int dispenseIndex = Random.Range(0, maps.Count);
        return maps[dispenseIndex];
    }
}
