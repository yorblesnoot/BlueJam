using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridTools
{
    static GameObject[,] gameMap;
    public static void SubmitTileMap(GameObject[,] map)
    {
        gameMap = map;
    }
    public static int[] VectorToMap (Vector3 toConvert)
    {
        int[] output = new int[2];
        output[0] = (int)Mathf.Floor(toConvert.x);
        output[1] = (int)Mathf.Floor(toConvert.z);
        return output;
    }

    public static Vector3 MapToVector (int x, int y, float height)
    {
        Vector3 newVector = new Vector3();
        newVector.x = (x+.5f);
        newVector.y = height;
        newVector.z = (y+.5f);
        return newVector;
    }

    public static GameObject VectorToTile(Vector3 toConvert)
    {
        int[] coords = VectorToMap(toConvert);
        return MapToTile(coords);
    }

    public static GameObject MapToTile(int[] coords)
    {
        try { return gameMap[coords[0], coords[1]]; }
        catch { return null; }
    }

    public static void ReportPositionChange(BattleUnit actor, BattleTileController newTile)
    {
        //report our location to the cell we're in
        newTile.unitContents = actor;
        GameObject oldTile = GridTools.VectorToTile(actor.transform.position);
        oldTile.GetComponent<BattleTileController>().unitContents = null;
    }

    public static void ReportPositionSwap(BattleUnit actor, BattleTileController newTile, BattleUnit secondActor)
    {
        //report locations when actors are switching places
        newTile.unitContents = actor;
        BattleTileController oldTile = GridTools.VectorToTile(actor.gameObject.transform.position).GetComponent<BattleTileController>();
        oldTile.unitContents = secondActor;
    }
}
