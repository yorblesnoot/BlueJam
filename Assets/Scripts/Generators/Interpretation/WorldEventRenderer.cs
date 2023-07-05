using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventRenderer : MonoBehaviour
{
    [SerializeField] GameObject bossEnemy;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject key;
    [SerializeField] GameObject remove;
    [SerializeField] GameObject item;
    [SerializeField] GameObject heal;

    readonly float eventHeight = .7f;
    public void RenderBoss(int x, int y)
    {
        if(x == 0)
        {
            x = 5; y = 5;
        }
        Instantiate(bossEnemy, MapTools.MapToVector(x, y, 1), Quaternion.identity);
    }
    public void RenderEnemies(List<Vector2Int> locations)
    {
        if (locations == null)
        {
            locations = new(){new Vector2Int ( 2, 2 )};
        }
        foreach (Vector2Int location in locations)
        {
            GameObject spawnedEnemy = Instantiate(enemy, MapTools.MapToVector(location[0], location[1], 1), Quaternion.identity);
        }
    }

    public void RenderEvents(string[,] eventMap)
    {
        Hashtable eventTable = new()
            {{ "k", key },
            { "r", remove },
            { "i", item },
            { "h", heal }};
        if(eventMap == null)
        {
            eventMap = new string[1,1];
        }
        for (int x = 0; x < eventMap.GetLength(0); x++)
        {
            for(int y = 0;  y < eventMap.GetLength(1); y++)
            {
                if (!string.IsNullOrEmpty(eventMap[x, y])) Instantiate((GameObject)eventTable[eventMap[x,y]], MapTools.MapToVector(x, y, eventHeight), Quaternion.identity);
            }
        }
    }
}
