using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapRenderer : MonoBehaviour
{
    [SerializeField] MapKey mapKey;
    public Hashtable mapHash;
#nullable enable
    public GameObject[,] RenderWorld(string[,]? worldMap)
    {
#nullable disable
        if (worldMap == null) return null;
        mapKey.Initialize();
        mapHash = mapKey.hashKey;
        int xMaster = worldMap.GetLength(0);
        int yMaster = worldMap.GetLength(1);
        GameObject[,] output = new GameObject[xMaster,yMaster];
        for(int x = 0; x < xMaster; x++)
        {
            for(int y = 0; y < yMaster; y++)
            {
                string tileKey = worldMap[x,y];
                if(tileKey != "x")
                {
                    output[x,y] = Instantiate((GameObject)mapHash[tileKey], MapTools.MapToVector(x,y,0), Quaternion.identity);
                }
            }
        }
        return output;
    }
}
