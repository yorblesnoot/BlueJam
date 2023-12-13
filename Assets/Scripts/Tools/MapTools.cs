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
        HashSet<Vector2Int> skipped = new();
        tileMap = new();
        List<float> xRange = mapTiles.Select(coord => coord.transform.position.x).ToList();
        xRange.Sort();
        List<float> yRange = mapTiles.Select(coord => coord.transform.position.z).ToList();
        yRange.Sort();

        int xDif = Mathf.CeilToInt(xRange.Last() - xRange.First()) + 1;
        int yDif = Mathf.CeilToInt(yRange.Last() - yRange.First()) + 1;
        Debug.Log($"x{xDif} y{yDif}");
        EvaluateTileRecursive(0, 0, xRange.First(), yRange.First());

        void EvaluateTileRecursive(int mapX, int mapY, float worldX, float worldY)
        {
            Debug.Log($"evaluating {mapX} {mapY} world coords {worldX} {worldY}");
            if (mapX >= xDif || mapY >= yDif) { Debug.Log("Outcome: Reached edge"); return; }
            Vector2Int incoming = new(mapX, mapY);
            if (tileMap.Contains(incoming) || skipped.Contains(incoming)) return;
            GameObject closestNode = mapTiles.Where(coord => (Mathf.Abs(worldX - coord.transform.position.x) < blockSize * emptyFactor)
            && (Mathf.Abs(worldY - coord.transform.position.z) < blockSize * emptyFactor)).FirstOrDefault();

            float nextWorldX = worldX;
            float nextWorldY = worldY;
            if (closestNode != null)
            {
                Vector2Int inc = new(mapX, mapY);
                tileMap.Add(inc, closestNode);
                nextWorldX = closestNode.transform.position.x;
                nextWorldY = closestNode.transform.position.z;
                Debug.Log("Outcome: Found");
            }
            else
            {
                skipped.Add(new(mapX, mapY));
                Debug.Log("Outcome: Skipped");
            }
            nextWorldX++;
            nextWorldY++;
            
            EvaluateTileRecursive(mapX + 1, mapY, nextWorldX, worldY);
            EvaluateTileRecursive(mapX, mapY + 1, worldX, nextWorldY);
        }

    }
}
