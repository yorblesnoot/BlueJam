using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class CellQuery : UnityEvent<int, int, string, GameObject> { }

public static class ZoneTargeter 
{
    //public CellQuery cellQuery;

    public static List<GameObject> ConvertMapRuleToTiles(string[,] targetData, Vector3 targetSource)
    {
        string terrainBlocked = "x";
        //find size of target data array
        List<GameObject> output = new List<GameObject>();

        //size of target data
        int xLength = targetData.GetLength(0);
        int yLength = targetData.GetLength(1);

        //centerpoint of target data in local coords
        int xCenterpoint = xLength/2;
        int yCenterpoint = yLength/2;

        //global coordinates of the ability source
        int[] sourceMap = GridTools.VectorToMap(targetSource);

        for(int x = 0; x < xLength; x++)
        {
            //y axis search
            for(int y = 0; y < yLength; y++)
            {
                
                //cellQuery?.Invoke(x, y, targetData[x,y], owner);
                if(targetData[x,y] != terrainBlocked)
                {
                    //difference between local centerpoint and local target cell
                    int xOffset = x - xCenterpoint;
                    int yOffset = y - yCenterpoint;

                    //get global coords for target cells
                    #nullable enable
                    GameObject? tile = GridTools.VectorToTile(GridTools.MapToVector(sourceMap[0] + xOffset, sourceMap[1] + yOffset, 0f));
                    #nullable disable

                    if(tile != null)
                    {
                        output.Add(tile);
                    }
                }
            }
        }
        return output;
    }

    //return true if areatargets found valid plays
    public static bool ValidPlay(GameObject tile, string tSource, List<CardClass> cardClass, string[,] aoeRule)
    {
        if (cardClass.Contains(CardClass.MOVE) || cardClass.Contains(CardClass.SUMMON))
        {
            if (TileIsValidTarget(tile, tSource, CardClass.MOVE) == true) return true;
            else return false;
        }
        else if(cardClass.Contains(CardClass.ATTACK) && AreaTargets(tile, tSource, CardClass.ATTACK, aoeRule).Count > 0)
        {
            return true;
        }
        else if (cardClass.Contains(CardClass.BUFF) && AreaTargets(tile, tSource, CardClass.BUFF, aoeRule).Count > 0)
        {
            return true;
        }
        else return false;
    }

    //return all valid targets in an aoe target based on the class, aoe size, and owner
    public static List<GameObject> AreaTargets(GameObject tile, string tSource, CardClass cardClass, string[,] aoeRule)
    {

        List<GameObject> checkCells = ConvertMapRuleToTiles(aoeRule, tile.transform.position);
        List<GameObject> outCells = new();
        if (checkCells.Count > 0)
        {
            for (int i = 0; i < checkCells.Count; i++)
            {
                if (TileIsValidTarget(checkCells[i], tSource, cardClass))
                {
                    GameObject cellContents = checkCells[i].GetComponent<BattleTileController>().unitContents;
                    if(cellContents != null) outCells.Add(cellContents);
                }
            }
            return outCells;
        }
        else return null;
    }

    public static bool TileIsValidTarget(GameObject tile, string tagOfSource, CardClass cardClass)
    {
        //logic to determine whether unit occupation makes the cell invalid
        #nullable enable
        GameObject? objTarget = tile.GetComponent<BattleTileController>().unitContents;
        #nullable disable

        string tagOfTarget;
        if (objTarget != null) tagOfTarget = objTarget.tag;
        else tagOfTarget = "Empty";
        
        if(cardClass == CardClass.ATTACK)
        {
            //if theyre both allies or enemies, invalid
            if(tagOfSource == tagOfTarget) return false;
            //ally cant target player
            else if(tagOfSource == "Ally" && tagOfTarget == "Player") return false;
            //player cant target ally
            else if(tagOfSource == "Player" && tagOfTarget == "Ally") return false;
            //attack can't target empty
            else if(tagOfTarget == "Empty") return false;
            else return true;
        }
        else if(cardClass == CardClass.MOVE)
        {
            //move can only target empty
            if(tagOfTarget == "Empty") return true;
            else return false;
        }

        else if(cardClass == CardClass.SUMMON)
        {
            //summon can only target empty
            if(tagOfTarget == "Empty") return true;
            else return false;
        }

        else if(cardClass == CardClass.BUFF)
        {
            //buff can only target similar
            if(tagOfSource == tagOfTarget) return true;
            //empty is no good
            else if(tagOfTarget == "Empty") return false;
            else if(tagOfSource == "Ally" && tagOfTarget == "Player") return true;
            else if(tagOfSource == "Player" && tagOfTarget == "Ally") return true;
            else return false;
        }
        else return false;        
    }
}
