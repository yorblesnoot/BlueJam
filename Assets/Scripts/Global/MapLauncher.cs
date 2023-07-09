using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLauncher : MonoBehaviour
{
    public Dictionary<Vector2Int, GameObject> map = new();
    public virtual void RequestMapReferences()
    {
        EventManager.requestMapReferences.Invoke(this);
        MapTools.SubmitTileMap(map);
    }

    public void SubmitMapReference(GameObject reference)
    {
        Vector2Int coords = MapTools.VectorToMap(reference.transform.position);
        map.Add(coords,reference);
    }
}
