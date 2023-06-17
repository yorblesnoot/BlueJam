using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    public static T Safe2DFind<T>(this T[,] source, int x, int y)
    {
        if (x >= 0 && y >= 0 && x < source.GetLength(0) && y < source.GetLength(1))
        {
            return source[x, y];
        }
        else return default;
    }

    public static List<T> Flatten<T>(this T[,] toFlatten)
    {
        List<T> result = new();
        for (int y = 0; y < toFlatten.GetLength(1); y++)
        {
            for (int x = 0; x < toFlatten.GetLength(0); x++)
            {
                result.Add(toFlatten[x,y]);
            }
        }
        return result;
    }
}
