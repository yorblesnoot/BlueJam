using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakWorldTiles : CorruptionElement
{
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] GameObject oneByOne;

    [SerializeField] WorldMapRenderer worldMapRenderer;
    [SerializeField] RunData runData;

    Dictionary<Vector2Int, GameObject> activeMap;
    int budget;
    public override void Activate(int budget)
    {
        activeMap = MapTools.tileMap.forward;
        this.budget = budget;
        EventManager.playerAtWorldLocation.AddListener((_) => CheckToBreak(activeMap, this.budget));
    }

    private void CheckToBreak(Dictionary<Vector2Int, GameObject> map, int budget)
    {
        int select = Random.Range(0, budget);
        if (select != 0) return;
        List<GameObject> availableTiles = new();
        foreach (var position in map.Keys)
        {
            float tileDistance = (position.MapToVector(0) - WorldPlayerControl.player.transform.position).magnitude;
            Debug.Log(tileDistance);
            if (tileDistance < minDistance || tileDistance > maxDistance) continue;
            availableTiles.Add(map[position]);
        }

        select = Random.Range(0, availableTiles.Count);
       BreakTile(availableTiles[select]);
    }

    private void BreakTile(GameObject tile)
    {
        //unrender is sending the unrendered tiles into the broken pool
        VFXMachine.PlayAtLocation("VoidBoom", tile.transform.position);
        Vector2Int targetPos = tile.transform.position.VectorToMap();
        Vector2Int globalPos = targetPos + WorldMapRenderer.spotlightGlobalOffset;
        runData.worldMap[globalPos.x, globalPos.y] = TerrainType.BROKEN;
        StartCoroutine(worldMapRenderer.UnrenderCell(targetPos));
        worldMapRenderer.RenderCell(TerrainType.BROKEN, targetPos);
    }
}
