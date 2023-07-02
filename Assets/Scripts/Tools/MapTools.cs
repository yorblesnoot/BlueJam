using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTools
{
    public static GameObject[,] gameMap;
    public static Vector2Int playerLocation;
    public static void SubmitTileMap(GameObject[,] map)
    {
        gameMap = map;
    }
    public static Vector2Int VectorToMap (Vector3 toConvert)
    {
        Vector2Int output = new()
        {
            x = (int)Mathf.Floor(toConvert.x),
            y = (int)Mathf.Floor(toConvert.z)
        };
        return output;
    }

    public static Vector3 MapToVector (int x, int y, float height)
    {
        Vector3 newVector = new()
        {
            x = (x + .5f),
            y = height,
            z = (y + .5f)
        };
        return newVector;
    }

    public static GameObject VectorToTile(Vector3 toConvert)
    {
        Vector2Int coords = VectorToMap(toConvert);
        return MapToTile(coords);
    }

    public static GameObject MapToTile(Vector2Int coords)
    {
        try { return gameMap[coords.x, coords.y]; }
        catch { return null; }
    }

    public static void ReportPositionChange(BattleUnit actor, BattleTileController newTile)
    {
        //report our location to the cell we're in
        GameObject oldTile = VectorToTile(actor.transform.position);
        oldTile.GetComponent<BattleTileController>().unitContents = null;
        newTile.unitContents = actor;
        ReportPlayer(actor, newTile.transform.position);
    }

    public static void ReportPlayer(BattleUnit actor, Vector3 position)
    {
        if (actor.gameObject.CompareTag("Player"))
        {
            playerLocation = VectorToMap(position);
        }
    }

    public static void ReportPositionSwap(BattleUnit actor, BattleTileController newTile, BattleUnit secondActor)
    {
        //report locations when actors are switching places
        newTile.unitContents = actor;
        BattleTileController oldTile = VectorToTile(actor.gameObject.transform.position).GetComponent<BattleTileController>();
        oldTile.unitContents = secondActor;
        ReportPlayer(actor, newTile.transform.position);
        ReportPlayer(secondActor, oldTile.transform.position);
    }
}
