using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPool", menuName = "ScriptableObjects/MapKit/SpawnPool")]
public class SpawnPool : ScriptableObject
{
    public List<int> spawnWeight;
    public List<GameObject> spawnUnits;

    public List<GameObject> staticSpawns;
}
