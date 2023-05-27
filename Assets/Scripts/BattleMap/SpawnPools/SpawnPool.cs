using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPool", menuName = "ScriptableObjects/SpawnPool")]
public class SpawnPool : ScriptableObject
{
    // Start is called before the first frame update
    public List<int> spawnWeight;
    public List<GameObject> spawnUnits;
}
