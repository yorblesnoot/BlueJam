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
        EventManager.playerAtWorldLocation.AddListener(CheckToBreak);
    }

    private void CheckToBreak(Vector2Int _)
    {
        int select = Random.Range(0, budget);
        if (select != 0) return;
        List<GameObject> availableTiles = new();
        foreach (var position in activeMap.Keys)
        {
            float tileDistance = (position.MapToVector(0) - WorldPlayerControl.player.transform.position).magnitude;
            //Debug.Log(tileDistance);
            if (tileDistance < minDistance || tileDistance > maxDistance) continue;
            availableTiles.Add(activeMap[position]);
        }

        select = Random.Range(0, availableTiles.Count);
        StartCoroutine(BreakTile(availableTiles[select]));
    }

    private IEnumerator BreakTile(GameObject tile)
    {
        //why is the animation making the tile disappear..? ~~~~~
        tile.GetComponent<Animator>().Play("Vibrate");
        yield return new WaitForSeconds(.5f);
        VFXMachine.PlayAtLocation("VoidUnderburst", tile.transform.position);
        Vector2Int targetPos = tile.transform.position.VectorToMap();
        Vector2Int globalPos = targetPos + WorldMapRenderer.spotlightGlobalOffset;
        
        StartCoroutine(worldMapRenderer.UnrenderCell(targetPos));
        runData.worldMap[globalPos.x, globalPos.y] = TerrainType.BROKEN;
        worldMapRenderer.RenderCell(TerrainType.BROKEN, targetPos);
    }
}
