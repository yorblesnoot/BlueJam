using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLauncher : MonoBehaviour
{
    public GameObject[,] map;
    public void RequestMapReferences()
    {
        map = new GameObject[20, 20];
        EventManager.requestMapReferences.Invoke(this);
        MapTools.SubmitTileMap(map);
    }

    public void SubmitMapReference(GameObject reference)
    {
        Vector2Int coords = MapTools.VectorToMap(reference.transform.position);
        map[coords[0], coords[1]] = reference;
    }
}
