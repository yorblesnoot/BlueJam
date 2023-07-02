using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListHelper
{
    public static bool RemoveCoordinates(this List<Vector2Int> list, Vector2Int toRemove)
    {
        foreach (Vector2Int entry in list)
        {
            if(entry == toRemove)
            {
                list.Remove(entry);
                return true;
            }
        }
        return false;
    }
}
