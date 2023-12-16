using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWorldTiles : CorruptionElement
{
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;

    [SerializeField] WorldMapRenderer worldMapRenderer;
    [SerializeField] RunData runData;

    [Range(1, 100)][SerializeField] int budget;
    public override void Activate(int budget)
    {
        Dictionary<Vector2Int, GameObject>  activeMap = MapTools.tileMap.forward;
        int select = Random.Range(0, 100);
        if (select > budget) return;
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
        Animator tileAni = tile.GetComponent<Animator>();
        tileAni.Play("Vibrate");
        yield return new WaitForSeconds(.5f);
        VFXMachine.PlayAtLocation("VoidUnderburst", tile.transform.position);
        Vector2Int targetPos = tile.transform.position.VectorToMap();
        Vector2Int globalPos = targetPos + WorldMapRenderer.spotlightGlobalOffset;
        
        StartCoroutine(worldMapRenderer.UnrenderCell(targetPos));
        runData.worldMap[globalPos.x, globalPos.y] = TerrainType.BROKEN;
        worldMapRenderer.RenderCell(TerrainType.BROKEN, targetPos);
    }
}
