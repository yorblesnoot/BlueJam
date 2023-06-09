using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListHelper
{
    public static bool RemoveCoordinates(this List<int[]> list, int[] toRemove)
    {
        foreach (int[] entry in list)
        {
            if(entry.SequenceEqual(toRemove))
            {
                list.Remove(entry);
                return true;
            }
        }
        return false;
    }
}
