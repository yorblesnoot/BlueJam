using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRenderer : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    public void RenderEvents(List<int[]> locations)
    {
        foreach (int[] location in locations)
        {
            GameObject spawnedEnemy = Instantiate(enemy, GridTools.MapToVector(location[0], location[1], 1), Quaternion.identity);
            WorldEnemy worldEnemy = spawnedEnemy.GetComponent<WorldEnemy>();
            worldEnemy.PullSpawnPool();
            worldEnemy.RegisterAggroZone();
        }
    }
}
