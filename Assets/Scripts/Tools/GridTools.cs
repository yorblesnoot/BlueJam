using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridTools
{
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
        Vector3 cleanedTarget = new Vector3(toConvert.x, 0f, toConvert.z);
        Collider[] colliders = Physics.OverlapSphere(cleanedTarget, .1f);
        if(colliders.Length > 0)
        {
            GameObject unit = colliders[0].gameObject;
            return unit;
        }
        else
        {
            return null;
        }
        
    }

    public static int GetMapDistance(Vector3 position1, Vector3 position2)
    {
        int[] coords1 = VectorToMap(position1);
        int[] coords2 = VectorToMap(position2);
        int distance = Mathf.Abs(coords1[0] - coords2[0]) + Mathf.Abs(coords1[1] - coords2[1]);
        return distance;
    }

    public static void ReportPositionChange(GameObject actor, GameObject newTile)
    {
        //report our location to the cell we're in
        newTile.GetComponent<BattleTileController>().unitContents = actor;
        GameObject oldTile = GridTools.VectorToTile(actor.transform.position);
        oldTile.GetComponent<BattleTileController>().unitContents = null;
    }

    public static void ReportPositionSwap(GameObject actor, GameObject newTile, GameObject secondActor)
    {
        //report locations when actors are switching places
        newTile.GetComponent<BattleTileController>().unitContents = actor;
        GameObject oldTile = GridTools.VectorToTile(actor.transform.position);
        oldTile.GetComponent<BattleTileController>().unitContents = secondActor;
    }
}
