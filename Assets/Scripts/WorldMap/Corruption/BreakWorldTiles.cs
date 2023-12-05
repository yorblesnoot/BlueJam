using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakWorldTiles : CorruptionElement
{
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] GameObject oneByOne;

    Dictionary<Vector2Int, GameObject> activeMap;
    int budget;
    public override void Activate(Dictionary<Vector2Int, GameObject> map, int budget)
    {
        activeMap = map;
        Debug.Log(map.Count);
        this.budget = budget;
        EventManager.playerAtWorldLocation.AddListener((_) => CheckToBreak(activeMap, this.budget));
    }

    private void CheckToBreak(Dictionary<Vector2Int, GameObject> map, int budget)
    {
        Debug.Log("check");
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
        StartCoroutine(BreakTile(availableTiles[select]));
    }

    private IEnumerator BreakTile(GameObject tile)
    {
        VFXMachine.PlayAtLocation("VoidBoom", tile.transform.position);
        yield return new WaitForSeconds(.1f);
        Instantiate(oneByOne, tile.transform.position, Quaternion.identity);
    }
}
