using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRenderer : MonoBehaviour
{
    [SerializeField] GameObject bossEnemy;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject key;
    [SerializeField] GameObject remove;
    [SerializeField] GameObject item;

    public void RenderBoss(int x, int y)
    {
        if(x == 0)
        {
            x = 5; y = 5;
        }
        Instantiate(bossEnemy, GridTools.MapToVector(x, y, 1), Quaternion.identity);
    }
    public void RenderEnemies(List<int[]> locations)
    {
        if (locations == null)
        {
            locations = new(){new int[] { 2, 2 }};
        }
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
        if(eventMap == null)
        {
            eventMap = new string[1,1];
        }
        for (int x = 0; x < eventMap.GetLength(0); x++)
        {
            for(int y = 0;  y < eventMap.GetLength(1); y++)
            {
                if (eventMap[x, y] != null) Instantiate((GameObject)eventTable[eventMap[x,y]], GridTools.MapToVector(x, y, 1), Quaternion.identity);
            }
        }
    }
}
