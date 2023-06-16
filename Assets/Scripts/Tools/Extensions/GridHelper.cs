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
}
