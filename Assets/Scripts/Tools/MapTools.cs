using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTools
{
    public static Dictionary<Vector2Int,GameObject> gameMap;
    public static void SubmitTileMap(Dictionary<Vector2Int, GameObject> map)
    {
        gameMap = map;
    }

    public static Vector2Int ObjectToMap(this GameObject obj)
    {
        return obj.transform.position.VectorToMap();
    }

    public static Vector2Int ObjectToMap(this BattleUnit obj)
    {
        return obj.transform.position.VectorToMap();
    }

    public static Vector2Int VectorToMap (this Vector3 toConvert)
    {
        Vector2Int output = new()
        {
            x = (int)Mathf.Floor(toConvert.x),
            y = (int)Mathf.Floor(toConvert.z)
        };
        return output;
    }

    public static Vector3 MapToVector (this Vector2Int coords, float height)
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
        return MapToTile(coords);
    }

    public static GameObject MapToTile(this Vector2Int coords)
    {
        if (!gameMap.ContainsKey(coords)) return null;
        GameObject tile = gameMap[coords];
        return tile;
    }

    public static void ReportPositionChange(BattleUnit actor, BattleTileController newTile)
    {
        //report our location to the cell we're in
        GameObject oldTile = VectorToTile(actor.transform.position);
        oldTile.GetComponent<BattleTileController>().unitContents = null;
        newTile.unitContents = actor;
        actor.gameObject.GetComponent<StencilControl>().ToggleStencil(newTile);
    }

    public static void ReportPositionSwap(BattleUnit actor, BattleTileController newTile, BattleUnit secondActor)
    {
        //report locations when actors are switching places
        BattleTileController oldTile = VectorToTile(actor.gameObject.transform.position).GetComponent<BattleTileController>();
        newTile.unitContents = actor;
        oldTile.unitContents = secondActor;
        actor.gameObject.GetComponent<StencilControl>().ToggleStencil(newTile);
        secondActor.gameObject.GetComponent<StencilControl>().ToggleStencil(oldTile);
    }
}
