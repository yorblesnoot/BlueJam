using System;
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
        for (int x = 0; x < toFlatten.GetLength(1); x++)
        {
            for (int y = 0; y < toFlatten.GetLength(0); y++)
            {
                result.Add(toFlatten[x,y]);
            }
        }
        return result;
    }

    public static T[,] Unflatten<T>(this List<T> toUnflatten)
    {
        int size = Convert.ToInt32(Mathf.Sqrt(toUnflatten.Count));
        T[,] result = new T[size, size];
        int index = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                result[x, y] = toUnflatten[index];
                index++;
            }
        }
        return result;
    }
}
