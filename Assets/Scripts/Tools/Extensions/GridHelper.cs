using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static List<Vector2Int> GetSurroundingCoordinates(this Vector2Int coordinates)
    {
        List<Vector2Int> surrounding = new();
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                surrounding.Add(new Vector2Int(x+coordinates.x, y+coordinates.y));
            }
        }
        return surrounding;
    }
    public static List<Vector2Int> GetAdjacentCoordinates(this Vector2Int coordinates)
    {
        return new List<Vector2Int>()
        {
            new Vector2Int(coordinates.x+1, coordinates.y),
            new Vector2Int(coordinates.x-1, coordinates.y),
            new Vector2Int(coordinates.x, coordinates.y+1),
            new Vector2Int(coordinates.x, coordinates.y-1),
        };
    }

    public static int GetLength(this Dictionary<Vector2Int, GameObject> map)
    {
        List<int> coords = map.Select(entry => entry.Key.x).ToList();
        return coords.Max()+1;
    }
}
