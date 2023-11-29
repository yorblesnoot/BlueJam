using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRelay", menuName = "ScriptableObjects/Singletons/SceneRelay")]
public class SceneRelay : ScriptableObject
{
    //enemy spawn info to convey from world to battle
    public List<GameObject> availableMaps;
    public SpawnPool spawnPool;
    public int enemyBudget;
    public int riftBudget;
    public bool bossEncounter;

    public Vector3 cameraPosition;

}
