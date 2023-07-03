using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRelay", menuName = "ScriptableObjects/Singletons/SceneRelay")]
public class SceneRelay : ScriptableObject
{
    //enemy spawn info to convey from world to battle
    public BiomePool availableMaps;
    public List<GameObject> staticSpawns;
    public List<GameObject> spawnUnits;
    public List<int> spawnWeights;
    public int enemyBudget;
    public bool bossEncounter;

    public Vector3 cameraPosition;

}
