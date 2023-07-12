using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEvent : MonoBehaviour
{
    public RunData runData;
    GameObject tile;
    private void Awake()
    {
        RegisterWithCell();
    }

    void RegisterWithCell()
    {
        tile = MapTools.VectorToTile(transform.position);
        tile.GetComponent<WorldEventHandler>().RegisterEvent(this);
    }

    public virtual void Activate()
    {
        //do whatever when the player enters the cell
        RemoveEvent();
    }

    void RemoveEvent()
    {
        Vector2Int coords = MapTools.VectorToMap(transform.position);
        coords += WorldMapRenderer.spotlightGlobalOffset;
        gameObject.SetActive(false);
    }
}
