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
        tile = GridTools.VectorToTile(transform.position);
        tile.GetComponent<WorldEventHandler>().RegisterEvent(this);
    }

    public virtual void Activate()
    {
        //do whatever when the player enters the cell
        RemoveEvent();
    }

    void RemoveEvent()
    {
        int[] coords = GridTools.VectorToMap(transform.position);
        runData.eventMap[coords[0], coords[1]] = null;
        gameObject.SetActive(false);
    }
}
