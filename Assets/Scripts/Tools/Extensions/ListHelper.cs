using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListHelper
{
    public static void TransferItemTo<T>(this  List<T> list1, List<T> list2, T item)
    {
        list1.Remove(item);
        list2.Add(item);
    }
}
