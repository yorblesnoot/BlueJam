using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MapTools
{
    public static ReversibleDictionary<Vector2Int, GameObject> tileMap = new();
    static ReversibleDictionary<BattleUnit, GameObject> unitTiles = new();
    public static void SubmitTileMap(List<GameObject> map)
    {
        BuildMap(map);
    }

    public static Vector2Int ToMap(this GameObject obj)
    {
        return tileMap[obj];
    }
    public static Vector2Int ToMap(this BattleTileController tile)
    {
        return tileMap[tile.gameObject];
    }

    public static BattleUnit OccupyingUnit(this GameObject obj)
    {
        if (!unitTiles.TryGetValue(obj, out BattleUnit battleUnit)) return null;
        return battleUnit;
    }
    public static BattleUnit OccupyingUnit(this BattleTileController tile) { return tile.gameObject.OccupyingUnit(); }

    public static BattleTileController OccupiedTile(this GameObject battleUnit)
    {
        return unitTiles[battleUnit.GetComponent<BattleUnit>()].GetComponent<BattleTileController>();
    }
    public static BattleTileController OccupiedTile(this BattleUnit battleUnit)
    {
        return unitTiles[battleUnit].GetComponent<BattleTileController>();
    }

    public static Vector2Int MapPosition(this BattleUnit battleUnit)
    {
        return battleUnit.OccupiedTile().ToMap();
    }
    public static Vector2Int MapPosition(this GameObject battleUnit)
    {
        return battleUnit.OccupiedTile().ToMap();
    }
    public static GameObject TileAtMapPosition(this Vector2Int coords)
    {
        if (!tileMap.Contains(coords)) return null;
        GameObject tile = tileMap[coords];
        return tile;
    }

    //WORLD MAP STUFF THAT SHOULD GO TO ANOTHER CLASS
    public static Vector2Int VectorToMap(this Vector3 toConvert)
    {
        Vector2Int output = new()
        {
            x = (int)Mathf.Floor(toConvert.x),
            y = (int)Mathf.Floor(toConvert.z)
        };
        return output;
    }
    public static Vector3 MapToVector(this Vector2Int coords, float height)
    {
        Vector3 newVector = new()
        {
            x = (coords.x + .5f),
            y = height,
            z = (coords.y + .5f)
        };
        return newVector;
    }
    public static GameObject VectorToTile(this Vector3 toConvert)
    {
        Vector2Int coords = VectorToMap(toConvert);
        return tileMap[coords];
    }

    public static void ReportPositionChange(BattleUnit actor, BattleTileController newTile)
    {
        //report our location to the cell we're in
        unitTiles.Remove(actor);
        unitTiles.Add(actor, newTile.gameObject);
        actor.gameObject.GetComponent<StencilControl>().ToggleStencil(newTile);
    }

    public static void ReportDepartureFromMap(BattleUnit actor)
    {
        unitTiles.Remove(actor);
    }

    public static void ReportPositionSwap(BattleUnit actor, BattleTileController newTile, BattleUnit secondActor)
    {
        //report locations when actors are switching places
        GameObject oldTile = unitTiles[actor];
        unitTiles.Remove(actor);
        unitTiles.Remove(secondActor);
        unitTiles.Add(actor, newTile.gameObject);
        unitTiles.Add(secondActor, oldTile);
        actor.gameObject.GetComponent<StencilControl>().ToggleStencil(newTile);
        secondActor.gameObject.GetComponent<StencilControl>().ToggleStencil(oldTile.GetComponent<BattleTileController>());
    }

    readonly static float blockSize = 1f;
    readonly static float emptyFactor = .5f;

    static void BuildMap(List<GameObject> mapTiles)
    {
        tileMap = new();
        Vector3 startTile = mapTiles.First().transform.position;
        EvaluateTileRecursive(0, 0, startTile.x, startTile.z);
        void EvaluateTileRecursive(int mapX, int mapY, float worldX, float worldY)
        {
            //Debug.Log($"evaluating {mapX} {mapY} world coords {worldX} {worldY}");
            Vector2Int incoming = new(mapX, mapY);
            GameObject closestNode = mapTiles.Where(coord => (Mathf.Abs(worldX - coord.transform.position.x) < blockSize * emptyFactor)
            && (Mathf.Abs(worldY - coord.transform.position.z) < blockSize * emptyFactor)).FirstOrDefault();

            if (closestNode == null) return;
            if (tileMap.Contains(closestNode)) return;

            Vector2Int inc = new(mapX, mapY);
            tileMap.Add(inc, closestNode);
            worldX = closestNode.transform.position.x;
            worldY = closestNode.transform.position.z;
            //Debug.Log("Outcome: Found");

            EvaluateTileRecursive(mapX + 1, mapY, worldX + 1, worldY);
            EvaluateTileRecursive(mapX, mapY + 1, worldX, worldY + 1);
            EvaluateTileRecursive(mapX - 1, mapY, worldX - 1, worldY);
            EvaluateTileRecursive(mapX, mapY - 1, worldX, worldY - 1);
        }
    }
}
