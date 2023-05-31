using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRenderer : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject key;
    [SerializeField] GameObject remove;
    [SerializeField] GameObject item;

    public void RenderEnemies(List<int[]> locations)
    {
        foreach (int[] location in locations)
        {
            GameObject spawnedEnemy = Instantiate(enemy, GridTools.MapToVector(location[0], location[1], 1), Quaternion.identity);
            WorldEnemy worldEnemy = spawnedEnemy.GetComponent<WorldEnemy>();
            worldEnemy.PullSpawnPool();
            worldEnemy.RegisterAggroZone();
        }
    }
    public void RenderEvents(string[,] eventMap)
    {
        Hashtable eventTable = new()
            {{ "k", key },
            { "r", remove },
            { "i", item }};
        for (int x = 0; x < eventMap.GetLength(0); x++)
        {
            for(int y = 0;  y < eventMap.GetLength(1); y++)
            {
                if (eventMap[x, y] != null) Instantiate((GameObject)eventTable[eventMap[x,y]], GridTools.MapToVector(x, y, 1), Quaternion.identity);
            }
        }
    }
}
